using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models.DTOs
{
    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "ID danh mục không được để trống")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Slug không được để trống")]
        [StringLength(100, ErrorMessage = "Slug không được vượt quá 100 ký tự")]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug phải chứa chữ thường, số và gạch nối")]
        public string Slug { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "ParentId phải là giá trị dương")]
        public int? ParentId { get; set; }

        public bool IsActive { get; set; }
    }
}
