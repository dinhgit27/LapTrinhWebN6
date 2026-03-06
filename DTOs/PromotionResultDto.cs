using FashionEcommerce.Models;

namespace FashionEcommerce.Models.DTOs
{
    // DTO chứa kết quả tính toán khuyến mãi
    public class PromotionResultDto
    {
        public int PromotionId { get; set; }

        public string PromotionName { get; set; } = null!;

        public DiscountType DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        // Số tiền giảm tính được
        public decimal DiscountAmount { get; set; }

        // Giá sau giảm
        public decimal FinalPrice { get; set; }

        // Độ ưu tiên của khuyến mãi
        public int? Priority { get; set; }

        // Thời gian bắt đầu
        public DateTime StartDate { get; set; }

        // Thời gian kết thúc
        public DateTime EndDate { get; set; }

        // Thông báo áp dụng
        public string Message { get; set; } = null!;
    }
}
