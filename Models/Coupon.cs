using System;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho mã giảm giá
    // Liên kết với chương trình khuyến mãi và người dùng sở hữu
    public class Coupon
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        [StringLength(50)]
        public string Code { get; set; }  // Mã giảm giá duy nhất

        [Required]
        public int UserId { get; set; }  // Khóa ngoại đến User sở hữu coupon

        [Required]
        public int PromotionId { get; set; }  // Khóa ngoại đến Promotion

        public bool? IsUsed { get; set; }  // Coupon đã sử dụng chưa

        [Required]
        public DateTime ExpiryDate { get; set; }  // Ngày hết hạn

        public DateTime? CreatedAt { get; set; }  // Thời gian tạo

        // Navigation properties
        public virtual User User { get; set; }  // Người sở hữu coupon
        public virtual Promotion Promotion { get; set; }  // Chương trình khuyến mãi liên quan
    }
}