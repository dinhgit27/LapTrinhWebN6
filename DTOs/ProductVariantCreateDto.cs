using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.DTOs
{
    /// <summary>
    /// DTO để tạo mới biến thể sản phẩm
    /// </summary>
    public class ProductVariantCreateDto
    {
        [Required(ErrorMessage = "ID sản phẩm là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID sản phẩm phải là số dương")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "ID màu sắc là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID màu sắc phải là số dương")]
        public int ColorId { get; set; }

        [Required(ErrorMessage = "ID kích thước là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID kích thước phải là số dương")]
        public int SizeId { get; set; }

        [Required(ErrorMessage = "SKU là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU phải từ 3-50 ký tự")]
        public string Sku { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số không âm")]
        public int? Quantity { get; set; }

        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Điều chỉnh giá không hợp lệ")]
        public decimal? PriceModifier { get; set; }
    }
}
