using FashionEcommerce.DTOs;
using FashionEcommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly ILogger<PromotionsController> _logger;

        public PromotionsController(IPromotionService promotionService, ILogger<PromotionsController> logger)
        {
            _promotionService = promotionService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả chương trình khuyến mãi
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PromotionReadDto>>> GetAllPromotions()
        {
            try
            {
                var promotions = await _promotionService.GetAllPromotionsAsync();
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi lấy danh sách khuyến mãi: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi lấy danh sách khuyến mãi" });
            }
        }

        /// <summary>
        /// Lấy chương trình khuyến mãi theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PromotionReadDto>> GetPromotionById(int id)
        {
            try
            {
                var promotion = await _promotionService.GetPromotionByIdAsync(id);
                if (promotion == null)
                    return NotFound(new { message = $"Khuyến mãi ID {id} không tồn tại" });

                return Ok(promotion);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi lấy chi tiết khuyến mãi: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi lấy chi tiết khuyến mãi" });
            }
        }

        /// <summary>
        /// Lấy khuyến mãi áp dụng cho một sản phẩm
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<List<PromotionReadDto>>> GetApplicablePromotions(int productId)
        {
            try
            {
                if (productId <= 0)
                    return BadRequest(new { message = "ID sản phẩm không hợp lệ" });

                var promotions = await _promotionService.GetApplicablePromotionsAsync(productId);
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi lấy khuyến mãi cho sản phẩm: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi lấy khuyến mãi cho sản phẩm" });
            }
        }

        /// <summary>
        /// Tính toán discount cho một sản phẩm
        /// </summary>
        [HttpPost("calculate")]
        public async Task<ActionResult<PromotionCalculationResult>> CalculateProductDiscount(
            [FromBody] PromotionCalculationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _promotionService.CalculateProductDiscountAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi tính toán discount: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Tính toán discount cho giỏ hàng
        /// </summary>
        [HttpPost("calculate-cart")]
        public async Task<ActionResult<CartDiscountResultDto>> CalculateCartDiscount(
            [FromBody] CartPromotionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _promotionService.CalculateCartDiscountAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi tính toán discount giỏ hàng: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của khuyến mãi
        /// </summary>
        [HttpGet("validate/{promotionId}")]
        public async Task<ActionResult<PromotionValidationDto>> ValidatePromotion(int promotionId)
        {
            try
            {
                if (promotionId <= 0)
                    return BadRequest(new { message = "ID khuyến mãi không hợp lệ" });

                var validation = await _promotionService.ValidatePromotionAsync(promotionId);
                return Ok(validation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi kiểm tra khuyến mãi: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi kiểm tra khuyến mãi" });
            }
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của mã coupon
        /// </summary>
        [HttpGet("coupon/validate/{code}")]
        public async Task<ActionResult<CouponValidationDto>> ValidateCoupon(
            string code,
            [FromQuery] int? userId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return BadRequest(new { message = "Mã coupon không được trống" });

                var validation = await _promotionService.ValidateCouponAsync(code, userId);
                return Ok(validation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi kiểm tra coupon: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi kiểm tra coupon" });
            }
        }

        /// <summary>
        /// Lấy khuyến mãi theo điều kiện
        /// </summary>
        /// <remarks>
        /// Ví dụ:
        /// - field=TotalAmount, operator=>=, value=500000
        /// - field=Category, operator==, value=Electronics
        /// </remarks>
        [HttpGet("by-condition")]
        public async Task<ActionResult<List<PromotionReadDto>>> GetPromotionsByCondition(
            [FromQuery] string field,
            [FromQuery] string conditionOperator,
            [FromQuery] string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(conditionOperator) || string.IsNullOrWhiteSpace(value))
                    return BadRequest(new { message = "Các tham số field, operator, value là bắt buộc" });

                var promotions = await _promotionService.GetPromotionsByConditionAsync(field, conditionOperator, value);
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi lấy khuyến mãi theo điều kiện: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi lấy khuyến mãi theo điều kiện" });
            }
        }

        /// <summary>
        /// Tìm khuyến mãi tốt nhất cho một sản phẩm
        /// </summary>
        [HttpPost("best-promotion")]
        public async Task<ActionResult<AppliedPromotionDto>> GetBestPromotion(
            [FromQuery] int productId,
            [FromQuery] decimal basePrice,
            [FromQuery] int quantity = 1)
        {
            try
            {
                if (productId <= 0)
                    return BadRequest(new { message = "ID sản phẩm không hợp lệ" });

                if (basePrice <= 0)
                    return BadRequest(new { message = "Giá sản phẩm phải là số dương" });

                if (quantity <= 0)
                    return BadRequest(new { message = "Số lượng phải là số dương" });

                var promotions = await _promotionService.GetApplicablePromotionsAsync(productId);
                var bestPromotion = await _promotionService.GetBestPromotionAsync(promotions, basePrice, quantity);

                if (bestPromotion == null)
                    return NotFound(new { message = "Không có khuyến mãi phù hợp" });

                return Ok(bestPromotion);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi tìm khuyến mãi tốt nhất: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi tìm khuyến mãi tốt nhất" });
            }
        }

        /// <summary>
        /// Tính toán tiết kiệm được cho một sản phẩm
        /// </summary>
        [HttpGet("savings")]
        public async Task<ActionResult<dynamic>> GetSavings(
            [FromQuery] int productId,
            [FromQuery] int quantity = 1,
            [FromQuery] string? couponCode = null)
        {
            try
            {
                if (productId <= 0)
                    return BadRequest(new { message = "ID sản phẩm không hợp lệ" });

                if (quantity <= 0)
                    return BadRequest(new { message = "Số lượng phải là số dương" });

                var request = new PromotionCalculationRequest
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CouponCode = couponCode
                };

                var result = await _promotionService.CalculateProductDiscountAsync(request);

                return Ok(new
                {
                    productId = result.ProductId,
                    originalPrice = result.OriginalPrice,
                    quantity = result.Quantity,
                    originalTotal = result.OriginalTotal,
                    savings = result.DiscountAmount,
                    savingsPercentage = result.OriginalTotal > 0 ? (result.DiscountAmount / result.OriginalTotal) * 100 : 0,
                    finalPrice = result.FinalPrice,
                    finalTotal = result.FinalTotal,
                    appliedPromotions = result.AppliedPromotions?.Select(p => new
                    {
                        name = p.PromotionName,
                        discount = p.DiscountAmount
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi tính toán tiết kiệm: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
