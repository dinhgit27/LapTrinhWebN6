using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.DTOs
{
    /// <summary>
    /// DTO để cập nhật sản phẩm
    /// </summary>
    public class ProductUpdateDto
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên sản phẩm phải từ 3-200 ký tự")]
        public string? Name { get; set; }

        [StringLength(200, ErrorMessage = "Slug tối đa 200 ký tự")]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug chỉ chứa chữ thường, số và dấu gạch")]
        public string? Slug { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số dương")]
        public decimal? Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Danh mục phải là số dương")]
        public int? CategoryId { get; set; }

        [StringLength(500, ErrorMessage = "URL ảnh tối đa 500 ký tự")]
        [Url(ErrorMessage = "URL ảnh không hợp lệ")]
        public string? Thumbnail { get; set; }

        public bool? IsActive { get; set; }
    }
}
