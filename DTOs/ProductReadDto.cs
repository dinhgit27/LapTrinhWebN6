namespace FashionEcommerce.Models.DTOs
{
    public class ProductReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string? Thumbnail { get; set; }

        public bool IsActive { get; set; }

        // Danh sách biến thể sản phẩm
        public ICollection<ProductVariantReadDto> ProductVariants { get; set; } = new List<ProductVariantReadDto>();

        // Thống kê
        public int VariantCount { get; set; }

        public int TotalStock { get; set; }

        public double AverageRating { get; set; }

        public int ReviewCount { get; set; }

        public int ImageCount { get; set; }
    }
}
