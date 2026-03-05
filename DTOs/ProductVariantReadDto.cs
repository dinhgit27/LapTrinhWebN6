namespace FashionEcommerce.DTOs
{
    /// <summary>
    /// DTO để trả về thông tin biến thể sản phẩm
    /// </summary>
    public class ProductVariantReadDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int ColorId { get; set; }

        public string? ColorName { get; set; }

        public string? ColorHexCode { get; set; }

        public int SizeId { get; set; }

        public string? SizeName { get; set; }

        public string? Sku { get; set; }

        public int? Quantity { get; set; }

        public decimal? PriceModifier { get; set; }

        /// <summary>
        /// Giá cuối cùng = Giá sản phẩm + Điều chỉnh giá
        /// </summary>
        public decimal FinalPrice { get; set; }
    }
}
