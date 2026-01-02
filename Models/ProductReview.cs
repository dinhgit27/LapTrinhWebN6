using System;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho đánh giá sản phẩm từ khách hàng
    // Lưu trữ điểm đánh giá, bình luận và liên kết với sản phẩm, người dùng, đơn hàng
    public class ProductReview
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        public int ProductId { get; set; }  // Khóa ngoại đến Product

        [Required]
        public int UserId { get; set; }  // Khóa ngoại đến User

        [Required]
        public int OrderId { get; set; }  // Khóa ngoại đến Order (đánh giá chỉ sau khi mua)

        [Range(1, 5)]
        public int? Rating { get; set; }  // Điểm đánh giá từ 1 đến 5

        [StringLength(1000)]
        public string Comment { get; set; }  // Bình luận chi tiết

        public DateTime? CreatedAt { get; set; }  // Thời gian tạo đánh giá

        // Navigation properties
        public virtual Product Product { get; set; }  // Sản phẩm được đánh giá
        public virtual User User { get; set; }  // Người đánh giá
        public virtual Order Order { get; set; }  // Đơn hàng liên quan
    }
}