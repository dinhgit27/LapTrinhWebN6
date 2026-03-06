using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models.DTOs
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Slug không được để trống")]
        [StringLength(100, ErrorMessage = "Slug không được vượt quá 100 ký tự")]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug phải chứa chữ thường, số và gạch nối")]
        public string Slug { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "ParentId phải là giá trị dương")]
        public int? ParentId { get; set; }  // ID danh mục cha (null nếu là danh mục gốc)

        public bool IsActive { get; set; } = true;  // Mặc định danh mục là hoạt động
    }
}
