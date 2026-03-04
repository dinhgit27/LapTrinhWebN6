using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.DTOs
{
    /// <summary>
    /// DTO để cập nhật danh mục sản phẩm
    /// </summary>
    public class CategoryUpdateDto
    {
        /// <summary>
        /// Tên danh mục
        /// </summary>
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục không vượt quá 100 ký tự")]
        public string Name { get; set; }

        /// <summary>
        /// Slug URL-friendly duy nhất
        /// </summary>
        [Required(ErrorMessage = "Slug không được để trống")]
        [StringLength(100, ErrorMessage = "Slug không vượt quá 100 ký tự")]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug chỉ chứa chữ thường, số và dấu gạch ngang")]
        public string Slug { get; set; }

        /// <summary>
        /// ID của danh mục cha (option - để tạo cấu trúc phân cấp)
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Danh mục có đang hoạt động không
        /// </summary>
        public bool IsActive { get; set; }
    }
}
