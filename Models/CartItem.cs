using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Thêm cái này để dùng [NotMapped]

namespace FashionEcommerce.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductVariantId { get; set; }

        [Required] // Nên để Required để tránh lỗi ép kiểu int? sang int
        public int Quantity { get; set; }

        // --- CÁC THUỘC TÍNH CẦN THÊM ĐỂ HẾT LỖI ĐỎ ---

        [NotMapped] // Dùng để tính toán, không lưu xuống bảng CartItems trong DB
        public string? ProductName { get; set; } 

        [NotMapped]
        public decimal Price { get; set; }

        [NotMapped]
        public string? Thumbnail { get; set; }

        // Sửa lỗi: 'CartItem' does not contain a definition for 'Total'
        [NotMapped]
        public decimal Total => Price * Quantity; 

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}