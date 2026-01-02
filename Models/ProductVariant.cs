using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho biến thể của sản phẩm (ví dụ: màu sắc, kích thước)
    // Mỗi biến thể có SKU riêng, số lượng tồn kho và giá điều chỉnh
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        public int ProductId { get; set; }  // Khóa ngoại đến Product

        [Required]
        public int ColorId { get; set; }  // Khóa ngoại đến MasterColor

        [Required]
        public int SizeId { get; set; }  // Khóa ngoại đến MasterSize

        [Required]
        [StringLength(50)]
        public string Sku { get; set; }  // Mã SKU duy nhất

        public int? Quantity { get; set; }  // Số lượng tồn kho

        public decimal? PriceModifier { get; set; }  // Điều chỉnh giá so với giá cơ bản của sản phẩm

        // Navigation properties
        public virtual Product? Product { get; set; }  // Sản phẩm gốc
        public virtual MasterColor? Color { get; set; }  // Màu sắc
        public virtual MasterSize? Size { get; set; }  // Kích thước
        public virtual ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();  // Xuất hiện trong giỏ hàng
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();  // Chi tiết đơn hàng
    }
}