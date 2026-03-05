using FashionEcommerce.Data;
using FashionEcommerce.DTOs;
using FashionEcommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VariantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả biến thể sản phẩm (không phân trang)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVariantReadDto>>> GetAllVariants()
        {
            var variants = await _context.ProductVariants
                .Include(pv => pv.Product)
                .Include(pv => pv.Color)
                .Include(pv => pv.Size)
                .ToListAsync();

            var variantDtos = variants.Select(pv => new ProductVariantReadDto
            {
                Id = pv.Id,
                ProductId = pv.ProductId,
                ColorId = pv.ColorId,
                ColorName = pv.Color?.Name,
                ColorHexCode = pv.Color?.HexCode,
                SizeId = pv.SizeId,
                SizeName = pv.Size?.Name,
                Sku = pv.Sku,
                Quantity = pv.Quantity,
                PriceModifier = pv.PriceModifier,
                FinalPrice = pv.Product?.Price + (pv.PriceModifier ?? 0) ?? 0
            }).ToList();

            return variantDtos;
        }

        /// <summary>
        /// Lấy danh sách biến thể sản phẩm theo màu sắc
        /// </summary>
        [HttpGet("by-color/{colorId}")]
        public async Task<ActionResult<IEnumerable<ProductVariantReadDto>>> GetVariantsByColor(int colorId)
        {
            // Kiểm tra xem màu sắc có tồn tại không
            var color = await _context.MasterColors.FindAsync(colorId);
            if (color == null)
            {
                return NotFound(new { message = $"Màu sắc với ID {colorId} không tồn tại" });
            }

            var variants = await _context.ProductVariants
                .Where(pv => pv.ColorId == colorId)
                .Include(pv => pv.Product)
                .Include(pv => pv.Color)
                .Include(pv => pv.Size)
                .ToListAsync();

            var variantDtos = variants.Select(pv => new ProductVariantReadDto
            {
                Id = pv.Id,
                ProductId = pv.ProductId,
                ColorId = pv.ColorId,
                ColorName = pv.Color?.Name,
                ColorHexCode = pv.Color?.HexCode,
                SizeId = pv.SizeId,
                SizeName = pv.Size?.Name,
                Sku = pv.Sku,
                Quantity = pv.Quantity,
                PriceModifier = pv.PriceModifier,
                FinalPrice = pv.Product?.Price + (pv.PriceModifier ?? 0) ?? 0
            }).ToList();

            return variantDtos;
        }

        /// <summary>
        /// Lấy danh sách biến thể sản phẩm theo kích thước
        /// </summary>
        [HttpGet("by-size/{sizeId}")]
        public async Task<ActionResult<IEnumerable<ProductVariantReadDto>>> GetVariantsBySize(int sizeId)
        {
            // Kiểm tra xem kích thước có tồn tại không
            var size = await _context.MasterSizes.FindAsync(sizeId);
            if (size == null)
            {
                return NotFound(new { message = $"Kích thước với ID {sizeId} không tồn tại" });
            }

            var variants = await _context.ProductVariants
                .Where(pv => pv.SizeId == sizeId)
                .Include(pv => pv.Product)
                .Include(pv => pv.Color)
                .Include(pv => pv.Size)
                .ToListAsync();

            var variantDtos = variants.Select(pv => new ProductVariantReadDto
            {
                Id = pv.Id,
                ProductId = pv.ProductId,
                ColorId = pv.ColorId,
                ColorName = pv.Color?.Name,
                ColorHexCode = pv.Color?.HexCode,
                SizeId = pv.SizeId,
                SizeName = pv.Size?.Name,
                Sku = pv.Sku,
                Quantity = pv.Quantity,
                PriceModifier = pv.PriceModifier,
                FinalPrice = pv.Product?.Price + (pv.PriceModifier ?? 0) ?? 0
            }).ToList();

            return variantDtos;
        }

        /// <summary>
        /// Lấy danh sách biến thể hết hàng
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ProductVariantReadDto>>> GetLowStockVariants(
            [FromQuery] int threshold = 10)
        {
            var variants = await _context.ProductVariants
                .Where(pv => pv.Quantity <= threshold)
                .Include(pv => pv.Product)
                .Include(pv => pv.Color)
                .Include(pv => pv.Size)
                .OrderBy(pv => pv.Quantity)
                .ToListAsync();

            var variantDtos = variants.Select(pv => new ProductVariantReadDto
            {
                Id = pv.Id,
                ProductId = pv.ProductId,
                ColorId = pv.ColorId,
                ColorName = pv.Color?.Name,
                ColorHexCode = pv.Color?.HexCode,
                SizeId = pv.SizeId,
                SizeName = pv.Size?.Name,
                Sku = pv.Sku,
                Quantity = pv.Quantity,
                PriceModifier = pv.PriceModifier,
                FinalPrice = pv.Product?.Price + (pv.PriceModifier ?? 0) ?? 0
            }).ToList();

            return variantDtos;
        }

        /// <summary>
        /// Cập nhật số lượng biến thể
        /// </summary>
        [HttpPatch("{id}/quantity")]
        public async Task<IActionResult> UpdateVariantQuantity(
            int id,
            [FromBody] QuantityUpdateDto updateDto)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null)
            {
                return NotFound(new { message = $"Biến thể với ID {id} không tồn tại" });
            }

            if (updateDto.Quantity < 0)
            {
                return BadRequest(new { message = "Số lượng không thể là số âm" });
            }

            variant.Quantity = updateDto.Quantity;
            _context.Entry(variant).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật số lượng thành công", quantity = variant.Quantity });
        }

        /// <summary>
        /// Cập nhật giá điều chỉnh của biến thể
        /// </summary>
        [HttpPatch("{id}/price-modifier")]
        public async Task<IActionResult> UpdatePriceModifier(
            int id,
            [FromBody] PriceModifierUpdateDto updateDto)
        {
            var variant = await _context.ProductVariants
                .Include(pv => pv.Product)
                .FirstOrDefaultAsync(pv => pv.Id == id);

            if (variant == null)
            {
                return NotFound(new { message = $"Biến thể với ID {id} không tồn tại" });
            }

            variant.PriceModifier = updateDto.PriceModifier;
            _context.Entry(variant).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var finalPrice = variant.Product?.Price + (variant.PriceModifier ?? 0) ?? 0;
            return Ok(new 
            { 
                message = "Cập nhật giá điều chỉnh thành công", 
                priceModifier = variant.PriceModifier,
                finalPrice = finalPrice
            });
        }

        /// <summary>
        /// Tìm kiếm biến thể theo múi tiêu chí (sản phẩm, màu, size)
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductVariantReadDto>>> SearchVariants(
            [FromQuery] int? productId = null,
            [FromQuery] int? colorId = null,
            [FromQuery] int? sizeId = null)
        {
            var query = _context.ProductVariants
                .Include(pv => pv.Product)
                .Include(pv => pv.Color)
                .Include(pv => pv.Size)
                .AsQueryable();

            if (productId.HasValue && productId > 0)
                query = query.Where(pv => pv.ProductId == productId);

            if (colorId.HasValue && colorId > 0)
                query = query.Where(pv => pv.ColorId == colorId);

            if (sizeId.HasValue && sizeId > 0)
                query = query.Where(pv => pv.SizeId == sizeId);

            var variants = await query.ToListAsync();

            var variantDtos = variants.Select(pv => new ProductVariantReadDto
            {
                Id = pv.Id,
                ProductId = pv.ProductId,
                ColorId = pv.ColorId,
                ColorName = pv.Color?.Name,
                ColorHexCode = pv.Color?.HexCode,
                SizeId = pv.SizeId,
                SizeName = pv.Size?.Name,
                Sku = pv.Sku,
                Quantity = pv.Quantity,
                PriceModifier = pv.PriceModifier,
                FinalPrice = pv.Product?.Price + (pv.PriceModifier ?? 0) ?? 0
            }).ToList();

            return variantDtos;
        }

        /// <summary>
        /// Lấy chi tiết một biến thể theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariantReadDto>> GetVariantById(int id)
        {
            var variant = await _context.ProductVariants
                .Include(pv => pv.Product)
                .Include(pv => pv.Color)
                .Include(pv => pv.Size)
                .FirstOrDefaultAsync(pv => pv.Id == id);

            if (variant == null)
            {
                return NotFound(new { message = $"Biến thể với ID {id} không tồn tại" });
            }

            var variantDto = new ProductVariantReadDto
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
                FinalPrice = variant.Product?.Price + (variant.PriceModifier ?? 0) ?? 0
            };

            return variantDto;
        }

        /// <summary>
        /// Xóa một biến thể theo ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null)
            {
                return NotFound(new { message = $"Biến thể với ID {id} không tồn tại" });
            }

            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Biến thể ID {id} đã bị xóa" });
        }

        /// <summary>
        /// Xóa nhiều biến thể một lúc
        /// </summary>
        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleVariants([FromBody] List<int> variantIds)
        {
            if (variantIds == null || variantIds.Count == 0)
            {
                return BadRequest(new { message = "Danh sách ID biến thể không được trống" });
            }

            var variants = await _context.ProductVariants
                .Where(pv => variantIds.Contains(pv.Id))
                .ToListAsync();

            if (variants.Count == 0)
            {
                return NotFound(new { message = "Không tìm thấy biến thể nào để xóa" });
            }

            _context.ProductVariants.RemoveRange(variants);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã xóa {variants.Count} biến thể" });
        }
    }

    /// <summary>
    /// DTO để cập nhật số lượng biến thể
    /// </summary>
    public class QuantityUpdateDto
    {
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO để cập nhật giá điều chỉnh của biến thể
    /// </summary>
    public class PriceModifierUpdateDto
    {
        public decimal PriceModifier { get; set; }
    }
}
