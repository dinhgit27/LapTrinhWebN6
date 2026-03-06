using FashionEcommerce.Models;

namespace FashionEcommerce.Models.DTOs
{
    // DTO cho yêu cầu tính toán khuyến mãi
    public class PromotionCalculationDto
    {
        public int ProductId { get; set; }

        public decimal BasePrice { get; set; }

        public int Quantity { get; set; }

        public int? UserId { get; set; }

        public string? CouponCode { get; set; }

        public decimal? TotalAmount { get; set; }  // Tổng tiền đơn hàng (nếu có)
    }
}
