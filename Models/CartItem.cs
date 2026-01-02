using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho sản phẩm trong giỏ hàng của người dùng
    // Lưu trữ tạm thời sản phẩm mà người dùng muốn mua
    public class CartItem
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        public int UserId { get; set; }  // Khóa ngoại đến User

        [Required]
        public int ProductId { get; set; }  // Khóa ngoại đến Product

        [Required]
        public int ProductVariantId { get; set; }  // Khóa ngoại đến ProductVariant

        public int? Quantity { get; set; }  // Số lượng sản phẩm trong giỏ

        // Navigation properties
        public virtual User User { get; set; }  // Người dùng sở hữu giỏ hàng
        public virtual Product Product { get; set; }  // Sản phẩm
        public virtual ProductVariant ProductVariant { get; set; }  // Biến thể cụ thể của sản phẩm
    }
}