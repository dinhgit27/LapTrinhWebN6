using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho danh mục sản phẩm
    // Hỗ trợ cấu trúc phân cấp (danh mục cha-con)
    public class Category
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // Tên danh mục

        [Required]
        [StringLength(100)]
        public string Slug { get; set; }  // Slug URL-friendly duy nhất

        public int? ParentId { get; set; }  // Khóa ngoại đến danh mục cha (self-referencing)

        public bool? IsActive { get; set; }  // Danh mục có đang hoạt động không

        // Navigation properties
        public virtual Category Parent { get; set; }  // Danh mục cha
        public virtual ICollection<Category> Children { get; set; } = new HashSet<Category>();  // Danh mục con
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();  // Sản phẩm trong danh mục
    }
}