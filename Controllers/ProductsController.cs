using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.DTOs;
using FashionEcommerce.Models.DTOs;
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

        // --- HELPER METHODS ---

        /// <summary>
        /// Map Product entity to ProductReadDto
        /// </summary>
        private ProductReadDto MapToProductReadDto(Product product)
        {
            return new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Thumbnail = product.Thumbnail,
                IsActive = product.IsActive,
                ProductVariants = product.ProductVariants != null ? product.ProductVariants
                    .Select(pv => MapToProductVariantReadDto(pv, product.Price))
                    .ToList() : new List<ProductVariantReadDto>(),
                VariantCount = product.ProductVariants?.Count ?? 0,
                TotalStock = product.ProductVariants?.Sum(pv => pv.Quantity) ?? 0,
                AverageRating = product.ProductReviews?.Any() == true 
                    ? (double)product.ProductReviews.Average(r => r.Rating) 
                    : 0,
                ReviewCount = product.ProductReviews?.Count ?? 0,
                ImageCount = product.ProductImages?.Count ?? 0
            };
        }

        /// <summary>
        /// Map ProductVariant entity to ProductVariantReadDto
        /// </summary>
        private ProductVariantReadDto MapToProductVariantReadDto(ProductVariant variant, decimal productPrice)
        {
            return new ProductVariantReadDto
            {
                Id = variant.Id,
                ProductId = variant.ProductId,
                ColorId = variant.ColorId,
                ColorName = variant.Color?.Name,
                ColorHexCode = variant.Color?.HexCode,
                SizeId = variant.SizeId,
                SizeName = variant.Size?.Name,
                Sku = variant.Sku,
                Quantity = variant.Quantity,
                PriceModifier = variant.PriceModifier,
                TotalPrice = productPrice + variant.PriceModifier,
                CartCount = variant.CartItems?.Count ?? 0
            };
        }

        // --- CRUD OPERATIONS FOR PRODUCTS ---

        /// <summary>
        /// GET: api/Products
        /// Lấy danh sách tất cả sản phẩm (chỉ admin)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProducts()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Color)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Size)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductReviews)
                    .ToListAsync();

                return Ok(products.Select(MapToProductReadDto).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách sản phẩm", error = ex.Message });
            }
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

        /// <summary>
        /// GET: api/Products/{id}
        /// Lấy thông tin chi tiết một sản phẩm cụ thể
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductReadDto>> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Color)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Size)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductReviews)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return NotFound(new { message = $"Không tìm thấy sản phẩm với ID {id}" });

                return Ok(MapToProductReadDto(product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin sản phẩm", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/Products/by-slug/{slug}
        /// Lấy sản phẩm theo Slug
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        public async Task<ActionResult<ProductReadDto>> GetProductBySlug(string slug)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(slug))
                    return BadRequest(new { message = "Slug không được để trống" });

                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Color)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Size)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductReviews)
                    .FirstOrDefaultAsync(p => p.Slug == slug.ToLower());

                if (product == null)
                    return NotFound(new { message = $"Không tìm thấy sản phẩm với slug '{slug}'" });

                return Ok(MapToProductReadDto(product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy sản phẩm theo slug", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/Products
        /// Tạo một sản phẩm mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductReadDto>> CreateProduct([FromBody] ProductCreateDto dto)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(new { message = "Tên sản phẩm không được để trống" });

                if (string.IsNullOrWhiteSpace(dto.Slug))
                    return BadRequest(new { message = "Slug không được để trống" });

                // Kiểm tra slug đã tồn tại
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Slug == dto.Slug.ToLower());

                if (existingProduct != null)
                    return BadRequest(new { message = $"Slug '{dto.Slug}' đã tồn tại" });

                // Kiểm tra danh mục tồn tại
                var category = await _context.Categories.FindAsync(dto.CategoryId);
                if (category == null)
                    return BadRequest(new { message = $"Danh mục với ID {dto.CategoryId} không tồn tại" });

                var product = new Product
                {
                    Name = dto.Name.Trim(),
                    Slug = dto.Slug.ToLower().Trim(),
                    Description = dto.Description?.Trim(),
                    Price = dto.Price,
                    CategoryId = dto.CategoryId,
                    Thumbnail = dto.Thumbnail?.Trim(),
                    IsActive = dto.IsActive
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Reload để lấy kèm dữ liệu liên quan
                var createdProduct = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Color)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Size)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductReviews)
                    .FirstOrDefaultAsync(p => p.Id == product.Id);

                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, MapToProductReadDto(createdProduct));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi database khi tạo sản phẩm", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo sản phẩm", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/Products/{id}
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            try
            {
                // Validate input
                if (id != dto.Id)
                    return BadRequest(new { message = "ID trong URL không khớp với ID trong body" });

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(new { message = "Tên sản phẩm không được để trống" });

                if (string.IsNullOrWhiteSpace(dto.Slug))
                    return BadRequest(new { message = "Slug không được để trống" });

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound(new { message = $"Không tìm thấy sản phẩm với ID {id}" });

                // Kiểm tra slug có đã được sử dụng bởi sản phẩm khác
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Slug == dto.Slug.ToLower() && p.Id != id);

                if (existingProduct != null)
                    return BadRequest(new { message = $"Slug '{dto.Slug}' đã tồn tại" });

                // Kiểm tra danh mục tồn tại
                if (dto.CategoryId != product.CategoryId)
                {
                    var category = await _context.Categories.FindAsync(dto.CategoryId);
                    if (category == null)
                        return BadRequest(new { message = $"Danh mục với ID {dto.CategoryId} không tồn tại" });
                }

                product.Name = dto.Name.Trim();
                product.Slug = dto.Slug.ToLower().Trim();
                product.Description = dto.Description?.Trim();
                product.Price = dto.Price;
                product.CategoryId = dto.CategoryId;
                product.Thumbnail = dto.Thumbnail?.Trim();
                product.IsActive = dto.IsActive;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                // Reload để trả về dữ liệu cập nhật
                var updatedProduct = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Color)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Size)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductReviews)
                    .FirstOrDefaultAsync(p => p.Id == id);

                return Ok(new
                {
                    message = "Cập nhật sản phẩm thành công",
                    data = MapToProductReadDto(updatedProduct)
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi database khi cập nhật sản phẩm", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật sản phẩm", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/Products/{id}
        /// Xóa một sản phẩm
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductVariants)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return NotFound(new { message = $"Không tìm thấy sản phẩm với ID {id}" });

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa sản phẩm thành công" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi database khi xóa sản phẩm", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa sản phẩm", error = ex.Message });
            }
        }

        /// <summary>
        /// PATCH: api/Products/{id}/toggle-active
        /// Bật/tắt trạng thái hoạt động của sản phẩm
        /// </summary>
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleProductActive(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Color)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Size)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductReviews)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return NotFound(new { message = $"Không tìm thấy sản phẩm với ID {id}" });

                product.IsActive = !product.IsActive;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Sản phẩm đã {(product.IsActive ? "kích hoạt" : "vô hiệu hóa")}",
                    data = MapToProductReadDto(product)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật trạng thái sản phẩm", error = ex.Message });
            }
        }
    }
}