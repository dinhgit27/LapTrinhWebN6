// Enum định nghĩa các loại giảm giá cho chương trình khuyến mãi
// Sử dụng trong model Promotion để chỉ định loại giảm giá (số tiền cố định hoặc phần trăm)
namespace FashionEcommerce.Models
{
    public enum DiscountType
    {
        FIXED_AMOUNT,  // Giảm giá số tiền cố định
        PERCENTAGE     // Giảm giá theo phần trăm
    }
}