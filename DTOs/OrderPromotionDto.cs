using FashionEcommerce.Models;

namespace FashionEcommerce.Models.DTOs
{
    // DTO cho kết quả tính toán khuyến mãi cho đơn hàng
    public class OrderPromotionDto
    {
        public decimal TotalBeforeDiscount { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal FinalTotal { get; set; }

        // Danh sách các khuyến mãi được áp dụng
        public List<PromotionResultDto> AppliedPromotions { get; set; } = new();

        // Coupon được áp dụng (nếu có)
        public string? AppliedCouponCode { get; set; }

        public string Message { get; set; } = null!;

        public bool Success { get; set; }
    }
}
