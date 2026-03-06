namespace FashionEcommerce.Models.DTOs
{
    public class CategoryReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public int? ParentId { get; set; }

        public bool IsActive { get; set; }

        // Thông tin parent (nếu có)
        public CategoryReadDto Parent { get; set; }

        // Danh sách category con
        public ICollection<CategoryReadDto> Children { get; set; } = new List<CategoryReadDto>();

        // Số lượng sản phẩm trong danh mục
        public int ProductCount { get; set; }
    }
}
