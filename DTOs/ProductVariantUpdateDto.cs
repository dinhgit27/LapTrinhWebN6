using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.DTOs
{
    /// <summary>
    /// DTO để cập nhật biến thể sản phẩm
    /// </summary>
    public class ProductVariantUpdateDto
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU phải từ 3-50 ký tự")]
        public string? Sku { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số không âm")]
        public int? Quantity { get; set; }

        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Điều chỉnh giá không hợp lệ")]
        public decimal? PriceModifier { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ID màu sắc phải là số dương")]
        public int? ColorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ID kích thước phải là số dương")]
        public int? SizeId { get; set; }
    }
}
