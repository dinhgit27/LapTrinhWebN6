using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.Models.DTOs;
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

        // --- HELPER METHODS ---

        /// <summary>
        /// Map ProductVariant entity to ProductVariantReadDto
        /// </summary>
        private ProductVariantReadDto MapToProductVariantReadDto(ProductVariant variant)
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
                TotalPrice = variant.Product != null ? variant.Product.Price + variant.PriceModifier : variant.PriceModifier,
                CartCount = variant.CartItems?.Count ?? 0
            };
        }

        // --- CRUD OPERATIONS FOR PRODUCT VARIANTS ---

        /// <summary>
        /// GET: api/Variants
        /// Lấy danh sách tất cả biến thể sản phẩm
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVariantReadDto>>> GetVariants()
        {
            try
            {
                var variants = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Include(v => v.Color)
                    .Include(v => v.Size)
                    .Include(v => v.CartItems)
                    .ToListAsync();

                return Ok(variants.Select(MapToProductVariantReadDto).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách biến thể", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/Variants/{id}
        /// Lấy thông tin chi tiết một biến thể cụ thể
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariantReadDto>> GetVariant(int id)
        {
            try
            {
                var variant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Include(v => v.Color)
                    .Include(v => v.Size)
                    .Include(v => v.CartItems)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (variant == null)
                    return NotFound(new { message = $"Không tìm thấy biến thể với ID {id}" });

                return Ok(MapToProductVariantReadDto(variant));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin biến thể", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/Variants/by-sku/{sku}
        /// Lấy biến thể theo SKU
        /// </summary>
        [HttpGet("by-sku/{sku}")]
        public async Task<ActionResult<ProductVariantReadDto>> GetVariantBySku(string sku)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sku))
                    return BadRequest(new { message = "SKU không được để trống" });

                var variant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Include(v => v.Color)
                    .Include(v => v.Size)
                    .Include(v => v.CartItems)
                    .FirstOrDefaultAsync(v => v.Sku == sku.ToUpper().Trim());

                if (variant == null)
                    return NotFound(new { message = $"Không tìm thấy biến thể với SKU '{sku}'" });

                return Ok(MapToProductVariantReadDto(variant));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy biến thể theo SKU", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/Variants/by-product/{productId}
        /// Lấy danh sách biến thể của một sản phẩm
        /// </summary>
        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductVariantReadDto>>> GetVariantsByProduct(int productId)
        {
            try
            {
                // Kiểm tra sản phẩm có tồn tại
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return NotFound(new { message = $"Không tìm thấy sản phẩm với ID {productId}" });

                var variants = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Include(v => v.Color)
                    .Include(v => v.Size)
                    .Include(v => v.CartItems)
                    .Where(v => v.ProductId == productId)
                    .ToListAsync();

                return Ok(variants.Select(MapToProductVariantReadDto).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy biến thể của sản phẩm", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/Variants
        /// Tạo một biến thể sản phẩm mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductVariantReadDto>> CreateVariant([FromBody] ProductVariantCreateDto dto)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(dto.Sku))
                    return BadRequest(new { message = "SKU không được để trống" });

                // Kiểm tra sản phẩm tồn tại
                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                    return BadRequest(new { message = $"Sản phẩm với ID {dto.ProductId} không tồn tại" });

                // Kiểm tra màu sắc tồn tại
                var color = await _context.MasterColors.FindAsync(dto.ColorId);
                if (color == null)
                    return BadRequest(new { message = $"Màu sắc với ID {dto.ColorId} không tồn tại" });

                // Kiểm tra kích thước tồn tại
                var size = await _context.MasterSizes.FindAsync(dto.SizeId);
                if (size == null)
                    return BadRequest(new { message = $"Kích thước với ID {dto.SizeId} không tồn tại" });

                // Kiểm tra SKU đã tồn tại
                var existingVariant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.Sku == dto.Sku.ToUpper().Trim());

                if (existingVariant != null)
                    return BadRequest(new { message = $"SKU '{dto.Sku}' đã tồn tại" });

                // Kiểm tra xem đã có biến thể với màu+size+productId này chưa
                var duplicateVariant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.ProductId == dto.ProductId 
                        && v.ColorId == dto.ColorId 
                        && v.SizeId == dto.SizeId);

                if (duplicateVariant != null)
                    return BadRequest(new { message = "Biến thể với màu và kích thước này đã tồn tại cho sản phẩm này" });

                var variant = new ProductVariant
                {
                    ProductId = dto.ProductId,
                    ColorId = dto.ColorId,
                    SizeId = dto.SizeId,
                    Sku = dto.Sku.ToUpper().Trim(),
                    Quantity = dto.Quantity,
                    PriceModifier = dto.PriceModifier
                };

                _context.ProductVariants.Add(variant);
                await _context.SaveChangesAsync();

                // Reload để lấy kèm dữ liệu liên quan
                var createdVariant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Include(v => v.Color)
                    .Include(v => v.Size)
                    .Include(v => v.CartItems)
                    .FirstOrDefaultAsync(v => v.Id == variant.Id);

                return CreatedAtAction(nameof(GetVariant), new { id = createdVariant.Id }, MapToProductVariantReadDto(createdVariant));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi database khi tạo biến thể", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo biến thể", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/Variants/{id}
        /// Cập nhật thông tin biến thể sản phẩm
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVariant(int id, [FromBody] ProductVariantUpdateDto dto)
        {
            try
            {
                // Validate input
                if (id != dto.Id)
                    return BadRequest(new { message = "ID trong URL không khớp với ID trong body" });

                if (string.IsNullOrWhiteSpace(dto.Sku))
                    return BadRequest(new { message = "SKU không được để trống" });

                var variant = await _context.ProductVariants.FindAsync(id);
                if (variant == null)
                    return NotFound(new { message = $"Không tìm thấy biến thể với ID {id}" });

                // Kiểm tra sản phẩm tồn tại
                if (dto.ProductId != variant.ProductId)
                {
                    var product = await _context.Products.FindAsync(dto.ProductId);
                    if (product == null)
                        return BadRequest(new { message = $"Sản phẩm với ID {dto.ProductId} không tồn tại" });
                }

                // Kiểm tra màu sắc tồn tại
                var color = await _context.MasterColors.FindAsync(dto.ColorId);
                if (color == null)
                    return BadRequest(new { message = $"Màu sắc với ID {dto.ColorId} không tồn tại" });

                // Kiểm tra kích thước tồn tại
                var size = await _context.MasterSizes.FindAsync(dto.SizeId);
                if (size == null)
                    return BadRequest(new { message = $"Kích thước với ID {dto.SizeId} không tồn tại" });

                // Kiểm tra SKU có đã được sử dụng
                var existingVariant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.Sku == dto.Sku.ToUpper().Trim() && v.Id != id);

                if (existingVariant != null)
                    return BadRequest(new { message = $"SKU '{dto.Sku}' đã tồn tại" });

                // Kiểm tra xem đã có biến thể với màu+size+productId này chưa (ngoài variant hiện tại)
                var duplicateVariant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.Id != id
                        && v.ProductId == dto.ProductId 
                        && v.ColorId == dto.ColorId 
                        && v.SizeId == dto.SizeId);

                if (duplicateVariant != null)
                    return BadRequest(new { message = "Biến thể với màu và kích thước này đã tồn tại cho sản phẩm này" });

                variant.ProductId = dto.ProductId;
                variant.ColorId = dto.ColorId;
                variant.SizeId = dto.SizeId;
                variant.Sku = dto.Sku.ToUpper().Trim();
                variant.Quantity = dto.Quantity;
                variant.PriceModifier = dto.PriceModifier;

                _context.ProductVariants.Update(variant);
                await _context.SaveChangesAsync();

                // Reload để trả về dữ liệu cập nhật
                var updatedVariant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Include(v => v.Color)
                    .Include(v => v.Size)
                    .Include(v => v.CartItems)
                    .FirstOrDefaultAsync(v => v.Id == id);

                return Ok(new
                {
                    message = "Cập nhật biến thể thành công",
                    data = MapToProductVariantReadDto(updatedVariant)
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi database khi cập nhật biến thể", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật biến thể", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/Variants/{id}
        /// Xóa một biến thể sản phẩm
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            try
            {
                var variant = await _context.ProductVariants
                    .Include(v => v.CartItems)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (variant == null)
                    return NotFound(new { message = $"Không tìm thấy biến thể với ID {id}" });

                // Kiểm tra biến thể có đang ở trong giỏ hàng
                if (variant.CartItems.Any())
                    return BadRequest(new { message = "Không thể xóa biến thể vì nó đang ở trong một số giỏ hàng" });

                _context.ProductVariants.Remove(variant);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa biến thể thành công" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi database khi xóa biến thể", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa biến thể", error = ex.Message });
            }
        }

        /// <summary>
        /// PATCH: api/Variants/{id}/update-quantity
        /// Cập nhật số lượng tồn kho của biến thể
        /// </summary>
        [HttpPatch("{id}/update-quantity")]
        public async Task<IActionResult> UpdateVariantQuantity(int id, [FromBody] dynamic request)
        {
            try
            {
                var variant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Include(v => v.Color)
                    .Include(v => v.Size)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (variant == null)
                    return NotFound(new { message = $"Không tìm thấy biến thể với ID {id}" });

                int newQuantity = request.quantity;

                if (newQuantity < 0)
                    return BadRequest(new { message = "Số lượng không được là số âm" });

                variant.Quantity = newQuantity;
                _context.ProductVariants.Update(variant);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Cập nhật số lượng thành công",
                    data = MapToProductVariantReadDto(variant)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật số lượng", error = ex.Message });
            }
        }

        /// <summary>
        /// PATCH: api/Variants/{id}/adjust-quantity
        /// Điều chỉnh số lượng (tăng/giảm) của biến thể
        /// </summary>
        [HttpPatch("{id}/adjust-quantity")]
        public async Task<IActionResult> AdjustVariantQuantity(int id, [FromBody] dynamic request)
        {
            try
            {
                var variant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Include(v => v.Color)
                    .Include(v => v.Size)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (variant == null)
                    return NotFound(new { message = $"Không tìm thấy biến thể với ID {id}" });

                int adjustBy = request.adjustBy;

                int newQuantity = variant.Quantity + adjustBy;

                if (newQuantity < 0)
                    return BadRequest(new { message = "Số lượng sau điều chỉnh không được là số âm" });

                variant.Quantity = newQuantity;
                _context.ProductVariants.Update(variant);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Điều chỉnh số lượng thành công",
                    data = MapToProductVariantReadDto(variant)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi điều chỉnh số lượng", error = ex.Message });
            }
        }
    }
}
