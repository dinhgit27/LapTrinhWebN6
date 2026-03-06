using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.Models.DTOs;
using FashionEcommerce.Services;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.Services
{
    /// <summary>
    /// Service tính toán và áp dụng khuyến mãi (Promotion Engine)
    /// </summary>
    public class PromotionService : IPromotionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PromotionService> _logger;

        public PromotionService(ApplicationDbContext context, ILogger<PromotionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách khuyến mãi áp dụng được cho sản phẩm
        /// </summary>
        public async Task<List<Promotion>> GetApplicablePromotionsForProduct(int productId)
        {
            try
            {
                var now = DateTime.UtcNow;

                var promotions = await _context.ProductPromotions
                    .Include(pp => pp.Promotion)
                        .ThenInclude(p => p.PromotionConditions)
                    .Where(pp => pp.ProductId == productId)
                    .Select(pp => pp.Promotion)
                    .Where(p => p.IsActive == true
                        && p.StartDate <= now
                        && p.EndDate >= now)
                    .OrderByDescending(p => p.Priority)
                    .ToListAsync();

                _logger.LogInformation($"Found {promotions.Count} applicable promotions for product {productId}");
                return promotions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting applicable promotions for product {productId}: {ex.Message}");
                return new List<Promotion>();
            }
        }

        /// <summary>
        /// Tính toán giảm giá cho một sản phẩm
        /// </summary>
        public async Task<decimal> CalculateDiscountForProduct(int productId, decimal basePrice, int quantity = 1)
        {
            try
            {
                var promotions = await GetApplicablePromotionsForProduct(productId);

                if (!promotions.Any())
                {
                    _logger.LogInformation($"No promotions found for product {productId}");
                    return 0;
                }

                // Lấy khuyến mãi có mức giảm cao nhất
                var bestPromotion = promotions.First();
                var totalPrice = basePrice * quantity;
                var discount = ApplyPromotionToPrice(totalPrice, bestPromotion);

                _logger.LogInformation($"Calculated discount for product {productId}: {discount}");
                return discount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating discount for product {productId}: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Lấy khuyến mãi có mức giảm cao nhất cho sản phẩm
        /// </summary>
        public async Task<Promotion?> GetBestPromotionForProduct(int productId, decimal basePrice)
        {
            try
            {
                var promotions = await GetApplicablePromotionsForProduct(productId);

                if (!promotions.Any())
                    return null;

                // Tính toán mức giảm cho mỗi khuyến mãi và trả về khuyến mãi tốt nhất
                var bestPromotion = promotions
                    .OrderByDescending(p => CalculateDiscountAmount(basePrice, p))
                    .FirstOrDefault();

                return bestPromotion;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting best promotion for product {productId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Xác thực coupon và lấy thông tin khuyến mãi
        /// </summary>
        public async Task<CouponValidationDto> ValidateCoupon(string couponCode, int userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(couponCode))
                {
                    return new CouponValidationDto
                    {
                        IsValid = false,
                        Message = "Mã coupon không được để trống"
                    };
                }

                var coupon = await _context.Coupons
                    .Include(c => c.Promotion)
                    .FirstOrDefaultAsync(c => c.Code == couponCode.ToUpper().Trim());

                if (coupon == null)
                {
                    return new CouponValidationDto
                    {
                        IsValid = false,
                        Message = "Mã coupon không tồn tại hoặc đã hết hạn"
                    };
                }

                // Kiểm tra coupon thuộc về user
                if (coupon.UserId != userId)
                {
                    return new CouponValidationDto
                    {
                        IsValid = false,
                        Message = "Mã coupon không thuộc về người dùng này"
                    };
                }

                // Kiểm tra coupon đã sử dụng chưa
                if (coupon.IsUsed == true)
                {
                    return new CouponValidationDto
                    {
                        IsValid = false,
                        Message = "Mã coupon đã được sử dụng"
                    };
                }

                // Kiểm tra ngày hết hạn
                if (coupon.ExpiryDate < DateTime.UtcNow)
                {
                    return new CouponValidationDto
                    {
                        IsValid = false,
                        Message = "Mã coupon đã hết hạn"
                    };
                }

                // Kiểm tra promotion còn hợp lệ không
                var isPromotionValid = await IsPromotionValid(coupon.PromotionId);
                if (!isPromotionValid)
                {
                    return new CouponValidationDto
                    {
                        IsValid = false,
                        Message = "Chương trình khuyến mãi của coupon này không còn hợp lệ"
                    };
                }

                // Coupon hợp lệ
                return new CouponValidationDto
                {
                    IsValid = true,
                    Message = "Mã coupon hợp lệ",
                    PromotionId = coupon.Promotion.Id,
                    PromotionName = coupon.Promotion.Name,
                    DiscountType = coupon.Promotion.DiscountType,
                    DiscountValue = coupon.Promotion.DiscountValue,
                    ExpiryDate = coupon.ExpiryDate,
                    StartDate = coupon.Promotion.StartDate,
                    EndDate = coupon.Promotion.EndDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating coupon {couponCode}: {ex.Message}");
                return new CouponValidationDto
                {
                    IsValid = false,
                    Message = $"Lỗi kiểm tra mã coupon: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Kiểm tra xem khuyến mãi có hợp lệ không
        /// </summary>
        public async Task<bool> IsPromotionValid(int promotionId, decimal? totalAmount = null)
        {
            try
            {
                var now = DateTime.UtcNow;

                var promotion = await _context.Promotions
                    .Include(p => p.PromotionConditions)
                    .FirstOrDefaultAsync(p => p.Id == promotionId);

                if (promotion == null)
                {
                    _logger.LogWarning($"Promotion {promotionId} not found");
                    return false;
                }

                // Kiểm tra hoạt động
                if (promotion.IsActive != true)
                {
                    _logger.LogInformation($"Promotion {promotionId} is not active");
                    return false;
                }

                // Kiểm tra ngày
                if (promotion.StartDate > now || promotion.EndDate < now)
                {
                    _logger.LogInformation($"Promotion {promotionId} is outside valid date range");
                    return false;
                }

                // Kiểm tra điều kiện (nếu có)
                if (promotion.PromotionConditions.Any() && totalAmount.HasValue)
                {
                    if (!ValidatePromotionConditions(promotion.PromotionConditions, totalAmount.Value))
                    {
                        _logger.LogInformation($"Promotion {promotionId} conditions not met");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating promotion {promotionId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Tính toán tổng giảm giá cho đơn hàng
        /// </summary>
        public async Task<OrderPromotionDto> CalculateOrderDiscount(
            List<(int ProductId, int Quantity, decimal UnitPrice)> orderItems,
            int? userId = null,
            string? couponCode = null)
        {
            try
            {
                var result = new OrderPromotionDto
                {
                    Success = false
                };

                if (!orderItems.Any())
                {
                    result.Message = "Đơn hàng không có sản phẩm";
                    return result;
                }

                result.TotalBeforeDiscount = orderItems.Sum(item => item.UnitPrice * item.Quantity);
                result.FinalTotal = result.TotalBeforeDiscount;

                // Kiểm tra và áp dụng coupon nếu có
                if (!string.IsNullOrWhiteSpace(couponCode) && userId.HasValue)
                {
                    var couponValidation = await ValidateCoupon(couponCode, userId.Value);

                    if (couponValidation.IsValid && couponValidation.PromotionId.HasValue)
                    {
                        var promotion = await GetPromotionById(couponValidation.PromotionId.Value);
                        if (promotion != null)
                        {
                            var discountAmount = ApplyPromotionToPrice(result.TotalBeforeDiscount, promotion);
                            result.TotalDiscount += discountAmount;
                            result.AppliedCouponCode = couponCode;

                            result.AppliedPromotions.Add(new PromotionResultDto
                            {
                                PromotionId = promotion.Id,
                                PromotionName = promotion.Name,
                                DiscountType = promotion.DiscountType,
                                DiscountValue = promotion.DiscountValue,
                                DiscountAmount = discountAmount,
                                FinalPrice = result.TotalBeforeDiscount - discountAmount,
                                Priority = promotion.Priority,
                                StartDate = promotion.StartDate,
                                EndDate = promotion.EndDate,
                                Message = "Coupon được áp dụng thành công"
                            });
                        }
                    }
                    else
                    {
                        result.Message = couponValidation.Message;
                    }
                }

                // Áp dụng khuyến mãi cho các sản phẩm
                foreach (var item in orderItems)
                {
                    var promotions = await GetApplicablePromotionsForProduct(item.ProductId);

                    if (promotions.Any())
                    {
                        var bestPromotion = promotions.First();
                        var itemTotal = item.UnitPrice * item.Quantity;
                        var itemDiscount = ApplyPromotionToPrice(itemTotal, bestPromotion);

                        // Chỉ áp dụng nếu không có coupon hoặc khuyến mãi tốt hơn
                        if (string.IsNullOrWhiteSpace(couponCode) || itemDiscount > result.TotalDiscount)
                        {
                            result.TotalDiscount = Math.Max(result.TotalDiscount, itemDiscount);

                            result.AppliedPromotions.Add(new PromotionResultDto
                            {
                                PromotionId = bestPromotion.Id,
                                PromotionName = bestPromotion.Name,
                                DiscountType = bestPromotion.DiscountType,
                                DiscountValue = bestPromotion.DiscountValue,
                                DiscountAmount = itemDiscount,
                                FinalPrice = itemTotal - itemDiscount,
                                Priority = bestPromotion.Priority,
                                StartDate = bestPromotion.StartDate,
                                EndDate = bestPromotion.EndDate,
                                Message = $"Khuyến mãi cho sản phẩm ID {item.ProductId}"
                            });
                        }
                    }
                }

                result.FinalTotal = result.TotalBeforeDiscount - result.TotalDiscount;
                result.Success = true;
                result.Message = $"Tính toán giảm giá thành công. Tổng giảm: {result.TotalDiscount:C}";

                _logger.LogInformation($"Order discount calculated: {result.TotalDiscount} from {result.TotalBeforeDiscount}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating order discount: {ex.Message}");
                return new OrderPromotionDto
                {
                    Success = false,
                    Message = $"Lỗi tính toán giảm giá: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Áp dụng khuyến mãi cho giá cơ bản
        /// </summary>
        public decimal ApplyPromotionToPrice(decimal basePrice, Promotion promotion)
        {
            try
            {
                if (promotion.DiscountType == DiscountType.FIXED_AMOUNT)
                {
                    // Giảm cố định
                    return Math.Min(promotion.DiscountValue, basePrice);
                }
                else if (promotion.DiscountType == DiscountType.PERCENTAGE)
                {
                    // Giảm theo phần trăm
                    var discount = basePrice * promotion.DiscountValue / 100;
                    return Math.Round(discount, 2);
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error applying promotion: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Lấy tất cả khuyến mãi đang hoạt động
        /// </summary>
        public async Task<List<Promotion>> GetActivePromotions()
        {
            try
            {
                var now = DateTime.UtcNow;

                var promotions = await _context.Promotions
                    .Where(p => p.IsActive == true
                        && p.StartDate <= now
                        && p.EndDate >= now)
                    .OrderByDescending(p => p.Priority)
                    .ToListAsync();

                return promotions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting active promotions: {ex.Message}");
                return new List<Promotion>();
            }
        }

        /// <summary>
        /// Lấy khuyến mãi theo ID
        /// </summary>
        public async Task<Promotion?> GetPromotionById(int promotionId)
        {
            try
            {
                var promotion = await _context.Promotions
                    .Include(p => p.PromotionConditions)
                    .FirstOrDefaultAsync(p => p.Id == promotionId);

                return promotion;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting promotion {promotionId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Đánh dấu coupon đã sử dụng
        /// </summary>
        public async Task<bool> MarkCouponAsUsed(string couponCode, int userId)
        {
            try
            {
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code == couponCode.ToUpper().Trim() && c.UserId == userId);

                if (coupon == null)
                {
                    _logger.LogWarning($"Coupon {couponCode} not found for user {userId}");
                    return false;
                }

                coupon.IsUsed = true;
                _context.Coupons.Update(coupon);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Coupon {couponCode} marked as used");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking coupon {couponCode} as used: {ex.Message}");
                return false;
            }
        }

        // --- HELPER METHODS ---

        /// <summary>
        /// Tính toán số tiền giảm cho một khuyến mãi
        /// </summary>
        private decimal CalculateDiscountAmount(decimal basePrice, Promotion promotion)
        {
            return ApplyPromotionToPrice(basePrice, promotion);
        }

        /// <summary>
        /// Kiểm tra điều kiện khuyến mãi
        /// </summary>
        private bool ValidatePromotionConditions(ICollection<PromotionCondition> conditions, decimal totalAmount)
        {
            try
            {
                foreach (var condition in conditions)
                {
                    if (!ValidateCondition(condition, totalAmount))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating promotion conditions: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra một điều kiện cụ thể
        /// </summary>
        private bool ValidateCondition(PromotionCondition condition, decimal totalAmount)
        {
            try
            {
                // Hiện tại chỉ hỗ trợ trường TotalAmount
                if (condition.Field.ToLower() != "totalamount")
                {
                    _logger.LogWarning($"Unsupported condition field: {condition.Field}");
                    return true;
                }

                if (!decimal.TryParse(condition.Value, out var conditionValue))
                {
                    _logger.LogWarning($"Invalid condition value: {condition.Value}");
                    return false;
                }

                return condition.Operator.ToLower() switch
                {
                    ">" => totalAmount > conditionValue,
                    ">=" => totalAmount >= conditionValue,
                    "<" => totalAmount < conditionValue,
                    "<=" => totalAmount <= conditionValue,
                    "=" or "==" => totalAmount == conditionValue,
                    "!=" => totalAmount != conditionValue,
                    _ => true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating single condition: {ex.Message}");
                return false;
            }
        }
    }
}
