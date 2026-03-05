using Microsoft.EntityFrameworkCore;
using FashionEcommerce.Data;
using FashionEcommerce.DTOs;
using FashionEcommerce.Models;

namespace FashionEcommerce.Services
{
    /// <summary>
    /// Service tính toán khuyến mãi (Promotion Engine)
    /// Xử lý logic tính toán, kiểm tra tính hợp lệ và áp dụng các chương trình khuyến mãi
    /// </summary>
    public interface IPromotionService
    {
        Task<List<PromotionReadDto>> GetAllPromotionsAsync();
        Task<PromotionReadDto?> GetPromotionByIdAsync(int promotionId);
        Task<List<PromotionReadDto>> GetApplicablePromotionsAsync(int productId);
        Task<PromotionCalculationResult> CalculateProductDiscountAsync(PromotionCalculationRequest request);
        Task<CartDiscountResultDto> CalculateCartDiscountAsync(CartPromotionRequest request);
        Task<PromotionValidationDto> ValidatePromotionAsync(int promotionId);
        Task<CouponValidationDto> ValidateCouponAsync(string couponCode, int? userId = null);
        Task<List<PromotionReadDto>> GetPromotionsByConditionAsync(string field, string conditionOperator, string value);
        Task<AppliedPromotionDto?> GetBestPromotionAsync(List<PromotionReadDto> promotions, decimal basePrice, int quantity);
    }

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
        /// Lấy tất cả chương trình khuyến mãi
        /// </summary>
        public async Task<List<PromotionReadDto>> GetAllPromotionsAsync()
        {
            var promotions = await _context.Promotions
                .Include(p => p.PromotionConditions)
                .OrderByDescending(p => p.Priority)
                .ThenByDescending(p => p.StartDate)
                .ToListAsync();

            return promotions.Select(p => MapToPromotionReadDto(p, DateTime.UtcNow)).ToList();
        }

        /// <summary>
        /// Lấy chương trình khuyến mãi theo ID
        /// </summary>
        public async Task<PromotionReadDto?> GetPromotionByIdAsync(int promotionId)
        {
            var promotion = await _context.Promotions
                .Include(p => p.PromotionConditions)
                .FirstOrDefaultAsync(p => p.Id == promotionId);

            if (promotion == null)
                return null;

            return MapToPromotionReadDto(promotion, DateTime.UtcNow);
        }

        /// <summary>
        /// Lấy danh sách khuyến mãi áp dụng cho một sản phẩm
        /// </summary>
        public async Task<List<PromotionReadDto>> GetApplicablePromotionsAsync(int productId)
        {
            var now = DateTime.UtcNow;

            // Lấy khuyến mãi trực tiếp áp dụng cho sản phẩm
            var directPromotions = await (from pp in _context.ProductPromotions
                                         join p in _context.Promotions on pp.PromotionId equals p.Id
                                         where pp.ProductId == productId &&
                                               p.IsActive == true &&
                                               p.StartDate <= now &&
                                               p.EndDate >= now
                                         select p)
                .Include(p => p.PromotionConditions)
                .OrderByDescending(p => p.Priority)
                .ToListAsync();

            return directPromotions.Select(p => MapToPromotionReadDto(p, now)).ToList();
        }

        /// <summary>
        /// Tính toán discount cho một sản phẩm
        /// </summary>
        public async Task<PromotionCalculationResult> CalculateProductDiscountAsync(PromotionCalculationRequest request)
        {
            try
            {
                // Lấy thông tin sản phẩm
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == request.ProductId);

                if (product == null)
                    throw new Exception($"Sản phẩm ID {request.ProductId} không tồn tại");

                var basePrice = product.Price;

                // Nếu có biến thể, cộng thêm điều chỉnh giá
                if (request.VariantId.HasValue)
                {
                    var variant = await _context.ProductVariants
                        .AsNoTracking()
                        .FirstOrDefaultAsync(pv => pv.Id == request.VariantId);

                    if (variant != null && variant.PriceModifier.HasValue)
                    {
                        basePrice += variant.PriceModifier.Value;
                    }
                }

                var originalTotal = basePrice * request.Quantity;

                // Lấy các khuyến mãi áp dụng
                var applicablePromotions = await GetApplicablePromotionsAsync(request.ProductId);

                // Áp dụng coupon nếu có
                CouponValidationDto? couponInfo = null;
                decimal couponDiscount = 0;

                if (!string.IsNullOrWhiteSpace(request.CouponCode))
                {
                    couponInfo = await ValidateCouponAsync(request.CouponCode);
                    if (couponInfo?.IsValid == true)
                    {
                        couponDiscount = CalculateDiscountAmount(
                            originalTotal,
                            couponInfo.DiscountType,
                            couponInfo.DiscountValue
                        );
                    }
                }

                // Tính tổng discount từ khuyến mãi
                var appliedPromotions = new List<AppliedPromotionDto>();
                decimal totalDiscountFromPromotions = 0;

                if (applicablePromotions.Any())
                {
                    var bestPromotion = await GetBestPromotionAsync(applicablePromotions, basePrice, request.Quantity);
                    if (bestPromotion != null)
                    {
                        totalDiscountFromPromotions = bestPromotion.DiscountAmount;
                        appliedPromotions.Add(bestPromotion);
                    }
                }

                // Tính tổng discount (chỉ áp dụng một trong hai: khuyến mãi sản phẩm hoặc coupon)
                decimal totalDiscount = Math.Max(totalDiscountFromPromotions, couponDiscount);
                decimal finalPrice = Math.Max(basePrice - (totalDiscount / request.Quantity), 0);
                decimal finalTotal = finalPrice * request.Quantity;

                return new PromotionCalculationResult
                {
                    ProductId = request.ProductId,
                    OriginalPrice = basePrice,
                    Quantity = request.Quantity,
                    OriginalTotal = originalTotal,
                    DiscountAmount = totalDiscount,
                    FinalPrice = finalPrice,
                    FinalTotal = finalTotal,
                    AppliedPromotions = appliedPromotions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi tính toán discount: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Tính toán discount cho giỏ hàng
        /// </summary>
        public async Task<CartDiscountResultDto> CalculateCartDiscountAsync(CartPromotionRequest request)
        {
            try
            {
                if (request.Items == null || !request.Items.Any())
                    throw new Exception("Giỏ hàng không có sản phẩm");

                decimal subtotalOriginal = 0;
                decimal productDiscounts = 0;
                var items = new List<ProductDiscountDto>();

                // Tính discount từng sản phẩm
                foreach (var item in request.Items)
                {
                    var result = await CalculateProductDiscountAsync(item);
                    var product = await _context.Products
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    subtotalOriginal += result.OriginalTotal;
                    productDiscounts += result.DiscountAmount;

                    items.Add(new ProductDiscountDto
                    {
                        ProductId = result.ProductId,
                        ProductName = product?.Name ?? "Sản phẩm",
                        OriginalPrice = result.OriginalPrice,
                        Quantity = result.Quantity,
                        OriginalTotal = result.OriginalTotal,
                        DiscountAmount = result.DiscountAmount,
                        DiscountPercentage = result.OriginalTotal > 0 ? (result.DiscountAmount / result.OriginalTotal) * 100 : 0,
                        FinalPrice = result.FinalPrice,
                        FinalTotal = result.FinalTotal,
                        AppliedPromotions = result.AppliedPromotions
                    });
                }

                // Xử lý coupon chung cho toàn giỏ
                decimal couponDiscount = 0;
                CouponAppliedDto? couponApplied = null;

                if (!string.IsNullOrWhiteSpace(request.CouponCode))
                {
                    var couponValidation = await ValidateCouponAsync(request.CouponCode, request.UserId);
                    if (couponValidation?.IsValid == true)
                    {
                        var afterProductDiscount = subtotalOriginal - productDiscounts;
                        couponDiscount = CalculateDiscountAmount(
                            afterProductDiscount,
                            couponValidation.DiscountType,
                            couponValidation.DiscountValue
                        );

                        couponApplied = new CouponAppliedDto
                        {
                            CouponId = couponValidation.CouponId,
                            Code = couponValidation.Code,
                            PromotionId = couponValidation.PromotionId,
                            PromotionName = couponValidation.PromotionName,
                            DiscountType = couponValidation.DiscountType,
                            DiscountValue = couponValidation.DiscountValue,
                            DiscountAmount = couponDiscount,
                            ExpiryDate = couponValidation.ExpiryDate,
                            IsExpired = couponValidation.IsExpired
                        };
                    }
                }

                var totalDiscount = productDiscounts + couponDiscount;
                var grandTotal = Math.Max(subtotalOriginal - totalDiscount, 0);

                return new CartDiscountResultDto
                {
                    Items = items,
                    SubtotalOriginal = subtotalOriginal,
                    ProductDiscounts = productDiscounts,
                    CouponDiscount = couponDiscount,
                    TotalDiscount = totalDiscount,
                    GrandTotal = grandTotal,
                    DiscountPercentage = subtotalOriginal > 0 ? (totalDiscount / subtotalOriginal) * 100 : 0,
                    CouponApplied = couponApplied
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi tính toán discount giỏ hàng: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của khuyến mãi
        /// </summary>
        public async Task<PromotionValidationDto> ValidatePromotionAsync(int promotionId)
        {
            var promotion = await _context.Promotions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == promotionId);

            if (promotion == null)
            {
                return new PromotionValidationDto
                {
                    PromotionId = promotionId,
                    IsValid = false,
                    Message = "Khuyến mãi không tồn tại"
                };
            }

            var now = DateTime.UtcNow;
            var isExpired = promotion.EndDate < now;
            var isNotStarted = promotion.StartDate > now;
            var isActive = promotion.IsActive == true;

            return new PromotionValidationDto
            {
                PromotionId = promotionId,
                IsValid = isActive && !isExpired && !isNotStarted,
                Message = isExpired ? "Khuyến mãi đã hết hạn" :
                         isNotStarted ? "Khuyến mãi chưa bắt đầu" :
                         !isActive ? "Khuyến mãi không hoạt động" :
                         "Khuyến mãi hợp lệ",
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsExpired = isExpired,
                IsNotStarted = isNotStarted,
                IsActive = isActive
            };
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của mã coupon
        /// </summary>
        public async Task<CouponValidationDto> ValidateCouponAsync(string couponCode, int? userId = null)
        {
            var coupon = await _context.Coupons
                .Include(c => c.Promotion)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == couponCode);

            if (coupon == null)
            {
                return new CouponValidationDto
                {
                    Code = couponCode,
                    IsValid = false,
                    Message = "Mã coupon không tồn tại"
                };
            }

            // Kiểm tra nếu có userId, coupon phải thuộc user đó
            if (userId.HasValue && coupon.UserId != userId)
            {
                return new CouponValidationDto
                {
                    CouponId = coupon.Id,
                    Code = coupon.Code,
                    IsValid = false,
                    Message = "Coupon không thuộc về người dùng này"
                };
            }

            var isExpired = coupon.ExpiryDate < DateTime.UtcNow;
            var isUsed = coupon.IsUsed == true;

            var promotionValidation = await ValidatePromotionAsync(coupon.PromotionId);

            var isValid = !isExpired && !isUsed && promotionValidation.IsValid;

            return new CouponValidationDto
            {
                CouponId = coupon.Id,
                Code = coupon.Code,
                IsValid = isValid,
                Message = isUsed ? "Coupon đã được sử dụng" :
                         isExpired ? "Coupon đã hết hạn" :
                         !promotionValidation.IsValid ? $"Khuyến mãi hôm không hợp lệ: {promotionValidation.Message}" :
                         "Coupon hợp lệ",
                IsExpired = isExpired,
                IsUsed = isUsed,
                ExpiryDate = coupon.ExpiryDate,
                PromotionId = coupon.PromotionId,
                PromotionName = coupon.Promotion?.Name ?? "",
                DiscountType = coupon.Promotion?.DiscountType.ToString() ?? "",
                DiscountValue = coupon.Promotion?.DiscountValue ?? 0
            };
        }

        /// <summary>
        /// Lấy khuyến mãi dựa trên điều kiện
        /// </summary>
        public async Task<List<PromotionReadDto>> GetPromotionsByConditionAsync(string field, string conditionOperator, string value)
        {
            var now = DateTime.UtcNow;

            var promotions = await (from p in _context.Promotions
                                   join pc in _context.PromotionConditions on p.Id equals pc.PromotionId
                                   where pc.Field == field && pc.Operator == conditionOperator &&
                                         p.IsActive == true &&
                                         p.StartDate <= now &&
                                         p.EndDate >= now
                                   select p)
                .Include(p => p.PromotionConditions)
                .Distinct()
                .OrderByDescending(p => p.Priority)
                .ToListAsync();

            return promotions.Select(p => MapToPromotionReadDto(p, now)).ToList();
        }

        /// <summary>
        /// Tìm khuyến mãi tốt nhất
        /// </summary>
        public async Task<AppliedPromotionDto?> GetBestPromotionAsync(List<PromotionReadDto> promotions, decimal basePrice, int quantity)
        {
            if (!promotions.Any())
                return null;

            AppliedPromotionDto? bestPromotion = null;
            decimal maxDiscount = 0;

            foreach (var promo in promotions.OrderByDescending(p => p.Priority))
            {
                var discount = CalculateDiscountAmount(
                    basePrice * quantity,
                    promo.DiscountType,
                    promo.DiscountValue
                );

                if (discount > maxDiscount)
                {
                    maxDiscount = discount;
                    bestPromotion = new AppliedPromotionDto
                    {
                        PromotionId = promo.Id,
                        PromotionName = promo.Name ?? "",
                        DiscountType = promo.DiscountType?.ToString() ?? "",
                        DiscountValue = promo.DiscountValue,
                        DiscountAmount = discount,
                        Priority = promo.Priority ?? 0
                    };
                }
            }

            return bestPromotion;
        }

        #region ==================== HỖ TRỢ ====================

        /// <summary>
        /// Tính toán số tiền giảm giá
        /// </summary>
        private decimal CalculateDiscountAmount(decimal totalAmount, string? discountType, decimal discountValue)
        {
            if (discountType == "PERCENTAGE")
            {
                return totalAmount * (discountValue / 100);
            }
            else if (discountType == "FIXED_AMOUNT")
            {
                return Math.Min(discountValue, totalAmount);
            }

            return 0;
        }

        /// <summary>
        /// Map Promotion entity thành PromotionReadDto
        /// </summary>
        private PromotionReadDto MapToPromotionReadDto(Promotion promotion, DateTime now)
        {
            var isExpired = promotion.EndDate < now;
            var isNotStarted = promotion.StartDate > now;
            var isActive = promotion.IsActive == true;

            return new PromotionReadDto
            {
                Id = promotion.Id,
                Name = promotion.Name,
                DiscountType = promotion.DiscountType.ToString(),
                DiscountValue = promotion.DiscountValue,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive,
                Priority = promotion.Priority,
                IsValid = isActive && !isExpired && !isNotStarted,
                PromotionConditions = promotion.PromotionConditions?.Select(pc => new PromotionConditionReadDto
                {
                    Id = pc.Id,
                    Field = pc.Field,
                    Operator = pc.Operator,
                    Value = pc.Value
                }).ToList()
            };
        }

        #endregion
    }
}
