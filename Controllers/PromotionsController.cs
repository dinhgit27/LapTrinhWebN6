using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.Models.DTOs;
using FashionEcommerce.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPromotionService _promotionService;
        private readonly ILogger<PromotionsController> _logger;

        public PromotionsController(
            ApplicationDbContext context,
            IPromotionService promotionService,
            ILogger<PromotionsController> logger)
        {
            _context = context;
            _promotionService = promotionService;
            _logger = logger;
        }

        // --- PROMOTION MANAGEMENT ---

        /// <summary>
        /// GET: api/Promotions
        /// Lấy danh sách tất cả khuyến mãi
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotion>>> GetPromotions()
        {
            try
            {
                var promotions = await _context.Promotions
                    .Include(p => p.PromotionConditions)
                    .Include(p => p.ProductPromotions)
                    .ToListAsync();

                return Ok(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting promotions: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách khuyến mãi", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/Promotions/active
        /// Lấy danh sách khuyến mãi đang hoạt động
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Promotion>>> GetActivePromotions()
        {
            try
            {
                var promotions = await _promotionService.GetActivePromotions();
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting active promotions: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy khuyến mãi đang hoạt động", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/Promotions/{id}
        /// Lấy thông tin chi tiết một khuyến mãi
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Promotion>> GetPromotion(int id)
        {
            try
            {
                var promotion = await _context.Promotions
                    .Include(p => p.PromotionConditions)
                    .Include(p => p.ProductPromotions)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (promotion == null)
                    return NotFound(new { message = $"Không tìm thấy khuyến mãi với ID {id}" });

                return Ok(promotion);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting promotion {id}: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin khuyến mãi", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/Promotions/by-product/{productId}
        /// Lấy khuyến mãi áp dụng cho sản phẩm
        /// </summary>
        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult> GetPromotionsByProduct(int productId)
        {
            try
            {
                var promotions = await _promotionService.GetApplicablePromotionsForProduct(productId);

                return Ok(new
                {
                    productId,
                    count = promotions.Count,
                    promotions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting promotions for product {productId}: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy khuyến mãi cho sản phẩm", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/Promotions
        /// Tạo một khuyến mãi mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreatePromotion([FromBody] Promotion promotion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(promotion.Name))
                    return BadRequest(new { message = "Tên khuyến mãi không được để trống" });

                if (promotion.StartDate >= promotion.EndDate)
                    return BadRequest(new { message = "Ngày kết thúc phải sau ngày bắt đầu" });

                _context.Promotions.Add(promotion);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created promotion {promotion.Id}");

                return CreatedAtAction(nameof(GetPromotion), new { id = promotion.Id }, promotion);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating promotion: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi tạo khuyến mãi", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/Promotions/{id}
        /// Cập nhật khuyến mãi
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromotion(int id, [FromBody] Promotion promotion)
        {
            try
            {
                if (id != promotion.Id)
                    return BadRequest(new { message = "ID không khớp" });

                var existingPromotion = await _context.Promotions.FindAsync(id);
                if (existingPromotion == null)
                    return NotFound(new { message = $"Không tìm thấy khuyến mãi với ID {id}" });

                existingPromotion.Name = promotion.Name;
                existingPromotion.DiscountType = promotion.DiscountType;
                existingPromotion.DiscountValue = promotion.DiscountValue;
                existingPromotion.StartDate = promotion.StartDate;
                existingPromotion.EndDate = promotion.EndDate;
                existingPromotion.IsActive = promotion.IsActive;
                existingPromotion.Priority = promotion.Priority;

                _context.Promotions.Update(existingPromotion);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated promotion {id}");

                return Ok(new { message = "Cập nhật khuyến mãi thành công", data = existingPromotion });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating promotion {id}: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi cập nhật khuyến mãi", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/Promotions/{id}
        /// Xóa khuyến mãi
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            try
            {
                var promotion = await _context.Promotions.FindAsync(id);
                if (promotion == null)
                    return NotFound(new { message = $"Không tìm thấy khuyến mãi với ID {id}" });

                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deleted promotion {id}");

                return Ok(new { message = "Xóa khuyến mãi thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting promotion {id}: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi xóa khuyến mãi", error = ex.Message });
            }
        }

        // --- PROMOTION ENGINE ---

        /// <summary>
        /// POST: api/Promotions/calculate-discount
        /// Tính toán giảm giá cho sản phẩm
        /// </summary>
        [HttpPost("calculate-discount")]
        public async Task<ActionResult> CalculateDiscount([FromBody] PromotionCalculationDto dto)
        {
            try
            {
                if (dto.BasePrice <= 0)
                    return BadRequest(new { message = "Giá gốc phải lớn hơn 0" });

                var discount = await _promotionService.CalculateDiscountForProduct(
                    dto.ProductId,
                    dto.BasePrice,
                    dto.Quantity);

                var finalPrice = (dto.BasePrice * dto.Quantity) - discount;

                return Ok(new
                {
                    productId = dto.ProductId,
                    basePrice = dto.BasePrice,
                    quantity = dto.Quantity,
                    totalBeforeDiscount = dto.BasePrice * dto.Quantity,
                    discountAmount = discount,
                    finalPrice = finalPrice,
                    discountPercent = dto.BasePrice > 0 ? (discount / (dto.BasePrice * dto.Quantity) * 100) : 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating discount: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi tính toán giảm giá", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/Promotions/validate-coupon
        /// Xác thực mã coupon
        /// </summary>
        [HttpPost("validate-coupon")]
        public async Task<ActionResult> ValidateCoupon([FromBody] dynamic request)
        {
            try
            {
                string couponCode = request.couponCode;
                int userId = request.userId;

                if (string.IsNullOrWhiteSpace(couponCode))
                    return BadRequest(new { message = "Mã coupon không được để trống" });

                var validation = await _promotionService.ValidateCoupon(couponCode, userId);

                if (!validation.IsValid)
                    return BadRequest(validation);

                return Ok(validation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating coupon: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi xác thực coupon", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/Promotions/calculate-order-discount
        /// Tính toán giảm giá cho đơn hàng
        /// </summary>
        [HttpPost("calculate-order-discount")]
        public async Task<ActionResult> CalculateOrderDiscount([FromBody] dynamic request)
        {
            try
            {
                var orderItems = request.orderItems as System.Collections.IEnumerable;
                int? userId = request.userId;
                string? couponCode = request.couponCode;

                if (orderItems == null || !orderItems.Cast<object>().Any())
                    return BadRequest(new { message = "Đơn hàng không có sản phẩm" });

                var items = new List<(int ProductId, int Quantity, decimal UnitPrice)>();

                foreach (var item in orderItems.Cast<dynamic>())
                {
                    items.Add(((int)item.productId, (int)item.quantity, (decimal)item.unitPrice));
                }

                var result = await _promotionService.CalculateOrderDiscount(items, userId, couponCode);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating order discount: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi tính toán giảm giá đơn hàng", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/Promotions/mark-coupon-used
        /// Đánh dấu coupon đã sử dụng
        /// </summary>
        [HttpPost("mark-coupon-used")]
        public async Task<ActionResult> MarkCouponAsUsed([FromBody] dynamic request)
        {
            try
            {
                string couponCode = request.couponCode;
                int userId = request.userId;

                var success = await _promotionService.MarkCouponAsUsed(couponCode, userId);

                if (!success)
                    return BadRequest(new { message = "Không thể đánh dấu coupon đã sử dụng" });

                return Ok(new { message = "Coupon đã được đánh dấu sử dụng" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking coupon as used: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi đánh dấu coupon", error = ex.Message });
            }
        }

        // --- COUPON MANAGEMENT ---

        /// <summary>
        /// GET: api/Promotions/coupons/{userId}
        /// Lấy danh sách coupon của người dùng
        /// </summary>
        [HttpGet("coupons/{userId}")]
        public async Task<ActionResult> GetUserCoupons(int userId)
        {
            try
            {
                var coupons = await _context.Coupons
                    .Include(c => c.Promotion)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                return Ok(new
                {
                    userId,
                    count = coupons.Count,
                    coupons
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting coupons for user {userId}: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách coupon", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/Promotions/assign-coupon
        /// Gán coupon cho người dùngPOST: api/Promotions/assign-coupon
        /// Gán coupon cho người dùng
        /// </summary>
        [HttpPost("assign-coupon")]
        public async Task<ActionResult> AssignCoupon([FromBody] Coupon coupon)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(coupon.Code))
                    return BadRequest(new { message = "Mã coupon không được để trống" });

                // Kiểm tra promotion tồn tại
                var promotion = await _context.Promotions.FindAsync(coupon.PromotionId);
                if (promotion == null)
                    return BadRequest(new { message = $"Khuyến mãi với ID {coupon.PromotionId} không tồn tại" });

                // Kiểm tra user tồn tại
                var user = await _context.Users.FindAsync(coupon.UserId);
                if (user == null)
                    return BadRequest(new { message = $"Người dùng với ID {coupon.UserId} không tồn tại" });

                // Kiểm tra mã coupon đã tồn tại
                var existingCoupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code == coupon.Code.ToUpper());

                if (existingCoupon != null)
                    return BadRequest(new { message = $"Mã coupon '{coupon.Code}' đã tồn tại" });

                coupon.Code = coupon.Code.ToUpper().Trim();
                coupon.CreatedAt = DateTime.UtcNow;
                coupon.IsUsed = false;

                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Assigned coupon {coupon.Code} to user {coupon.UserId}");

                return CreatedAtAction(nameof(GetUserCoupons), new { userId = coupon.UserId }, coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error assigning coupon: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi gán coupon", error = ex.Message });
            }
        }
    }
}
