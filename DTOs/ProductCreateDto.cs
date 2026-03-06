using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Slug không được để trống")]
        [StringLength(200, ErrorMessage = "Slug không được vượt quá 200 ký tự")]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug phải chứa chữ thường, số và gạch nối")]
        public string Slug { get; set; } = null!;

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0.01, 999999999.99, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Danh mục sản phẩm không được để trống")]
        public int CategoryId { get; set; }

        [StringLength(500, ErrorMessage = "URL ảnh đại diện không được vượt quá 500 ký tự")]
        public string? Thumbnail { get; set; }

        public bool IsActive { get; set; } = true;  // Mặc định sản phẩm là hoạt động
    }
}
