using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho điều kiện áp dụng của chương trình khuyến mãi
    // Xác định các tiêu chí để khuyến mãi có thể được áp dụng (ví dụ: tổng tiền tối thiểu)
    public class PromotionCondition
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        public int PromotionId { get; set; }  // Khóa ngoại đến Promotion

        [Required]
        [StringLength(50)]
        public string Field { get; set; }  // Trường dữ liệu để kiểm tra (ví dụ: "TotalAmount")

        [Required]
        [StringLength(20)]
        public string Operator { get; set; }  // Toán tử so sánh (ví dụ: ">", "<", "=")

        [Required]
        [StringLength(200)]
        public string Value { get; set; }  // Giá trị để so sánh

        // Navigation properties
        public virtual Promotion Promotion { get; set; }  // Tham chiếu đến Promotion sở hữu điều kiện này
    }
}