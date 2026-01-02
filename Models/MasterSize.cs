using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model master data cho kích thước sản phẩm
    // Lưu trữ các kích thước có sẵn (S, M, L, XL, etc.) để sử dụng trong ProductVariant
    public class MasterSize
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        [StringLength(20)]
        public string Name { get; set; }  // Tên kích thước (S, M, L, etc.)

        // Navigation properties
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new HashSet<ProductVariant>();  // Các biến thể sản phẩm có kích thước này
    }
}