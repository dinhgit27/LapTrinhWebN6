using System;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho thông báo gửi đến người dùng
    // Có thể là thông báo về đơn hàng, khuyến mãi, cập nhật hệ thống
    public class Notification
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        public int UserId { get; set; }  // Khóa ngoại đến User

        [Required]
        [StringLength(200)]
        public string Title { get; set; }  // Tiêu đề thông báo

        [Required]
        [StringLength(500)]
        public string Message { get; set; }  // Nội dung thông báo

        public bool? IsRead { get; set; }  // Thông báo đã đọc chưa

        [StringLength(50)]
        public string Type { get; set; }  // Loại thông báo (Order, Promotion, System, etc.)

        public DateTime? CreatedAt { get; set; }  // Thời gian tạo

        // Navigation properties
        public virtual User User { get; set; }  // Người nhận thông báo
    }
}