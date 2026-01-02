using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model master data cho màu sắc sản phẩm
    // Lưu trữ các màu có sẵn với mã hex để sử dụng trong ProductVariant
    public class MasterColor
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        [StringLength(50)]
        public string Name { get; set; }  // Tên màu (Red, Blue, etc.)

        [Required]
        [StringLength(10)]
        public string HexCode { get; set; }  // Mã màu hex (#FF0000, etc.)

        // Navigation properties
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new HashSet<ProductVariant>();  // Các biến thể sản phẩm có màu này
    }
}