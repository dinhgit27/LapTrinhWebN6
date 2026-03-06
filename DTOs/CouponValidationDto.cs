namespace FashionEcommerce.Models.DTOs
{
    // DTO cho kết quả xác thực coupon
    public class CouponValidationDto
    {
        public bool IsValid { get; set; }

        public string Message { get; set; } = null!;

        public int? PromotionId { get; set; }

        public string? PromotionName { get; set; }

        public DiscountType? DiscountType { get; set; }

        public decimal? DiscountValue { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
