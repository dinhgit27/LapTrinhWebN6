namespace FashionEcommerce.DTOs
{
    public class CartRequest
    {
        public int ProductId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}