using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho hình ảnh của sản phẩm
    // Lưu trữ URL hình ảnh và thứ tự hiển thị
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        public int ProductId { get; set; }  // Khóa ngoại đến Product

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; }  // URL của hình ảnh

        public int? SortOrder { get; set; }  // Thứ tự hiển thị hình ảnh

        // Navigation properties
        public virtual Product Product { get; set; }  // Sản phẩm sở hữu hình ảnh này
    }
}