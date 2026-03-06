namespace FashionEcommerce.Models.DTOs
{
    public class ProductVariantReadDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int ColorId { get; set; }

        public string? ColorName { get; set; }

        public string? ColorHexCode { get; set; }

        public int SizeId { get; set; }

        public string? SizeName { get; set; }

        public string Sku { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal PriceModifier { get; set; }

        // Giá cuối cùng = giá sản phẩm + price modifier
        public decimal TotalPrice { get; set; }

        // Số lượng sản phẩm này trong giỏ hàng (nếu cần)
        public int CartCount { get; set; }
    }
}
