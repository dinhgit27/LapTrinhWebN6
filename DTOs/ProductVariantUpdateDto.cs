using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models.DTOs
{
    public class ProductVariantUpdateDto
    {
        [Required(ErrorMessage = "ID biến thể không được để trống")]
        public int Id { get; set; }

        [Required(ErrorMessage = "ID sản phẩm không được để trống")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Màu sắc không được để trống")]
        public int ColorId { get; set; }

        [Required(ErrorMessage = "Kích thước không được để trống")]
        public int SizeId { get; set; }

        [Required(ErrorMessage = "SKU không được để trống")]
        [StringLength(50, ErrorMessage = "SKU không được vượt quá 50 ký tự")]
        public string Sku { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số dương")]
        public int Quantity { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Điều chỉnh giá phải hợp lệ")]
        public decimal PriceModifier { get; set; }
    }
}
