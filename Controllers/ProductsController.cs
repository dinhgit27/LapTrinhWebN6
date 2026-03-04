using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .ToListAsync();
        }

        // GET: api/Products/public
        // Public endpoint: filter & search products with paging and sorting
        [HttpGet("public")]
        public async Task<ActionResult> GetPublicProducts([
            FromQuery] string? q,
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int? colorId,
            [FromQuery] int? sizeId,
            [FromQuery] double? minRating,
            [FromQuery] bool? inStock,
            [FromQuery] string? sort,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Size)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductReviews)
                .Where(p => p.IsActive == true);

            if (!string.IsNullOrEmpty(q))
            {
                var qLower = q.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(qLower)
                    || (p.Description != null && p.Description.ToLower().Contains(qLower))
                    || p.Slug.ToLower().Contains(qLower));
            }

            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
            if (colorId.HasValue) query = query.Where(p => p.ProductVariants.Any(v => v.ColorId == colorId.Value));
            if (sizeId.HasValue) query = query.Where(p => p.ProductVariants.Any(v => v.SizeId == sizeId.Value));
            if (minRating.HasValue) query = query.Where(p => p.ProductReviews.Any() && p.ProductReviews.Average(r => r.Rating) >= minRating.Value);
            if (inStock.HasValue && inStock.Value) query = query.Where(p => p.ProductVariants.Any(v => v.Quantity > 0));

            switch ((sort ?? string.Empty).ToLower())
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "name_asc":
                    query = query.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.Name);
                    break;
                case "rating_desc":
                    query = query.OrderByDescending(p => p.ProductReviews.Any() ? p.ProductReviews.Average(r => r.Rating) : 0);
                    break;
                default:
                    query = query.OrderByDescending(p => p.Id);
                    break;
            }

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductPublicDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Price = p.Price,
                    Thumbnail = p.Thumbnail,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    AverageRating = p.ProductReviews.Any() ? (double)p.ProductReviews.Average(r => r.Rating) : 0,
                    ReviewCount = p.ProductReviews.Count(),
                    InStock = p.ProductVariants.Any(v => v.Quantity > 0)
                })
                .ToListAsync();

            return Ok(new { total, page, pageSize, items });
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Size)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}