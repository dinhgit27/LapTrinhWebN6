using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho chi tiết sản phẩm trong đơn hàng
    // Lưu trữ snapshot của sản phẩm tại thời điểm đặt hàng để tránh thay đổi giá/lý do khác
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        public int OrderId { get; set; }  // Khóa ngoại đến Order

        [Required]
        public int ProductVariantId { get; set; }  // Khóa ngoại đến ProductVariant

        [Required]
        [StringLength(200)]
        public string Snapshot_ProductName { get; set; }  // Tên sản phẩm tại thời điểm đặt

        [Required]
        [StringLength(50)]
        public string Snapshot_Sku { get; set; }  // SKU tại thời điểm đặt

        [StringLength(500)]
        public string Snapshot_Thumbnail { get; set; }  // URL ảnh tại thời điểm đặt

        [Required]
        public int Quantity { get; set; }  // Số lượng đặt

        [Required]
        public decimal UnitPrice { get; set; }  // Đơn giá tại thời điểm đặt

        // Navigation properties
        public virtual Order Order { get; set; }  // Đơn hàng chứa chi tiết này
        public virtual ProductVariant ProductVariant { get; set; }  // Biến thể sản phẩm
    }
}