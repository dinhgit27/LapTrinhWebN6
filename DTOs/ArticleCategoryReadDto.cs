using System;

namespace FashionEcommerce.DTOs
{//doc danh muc
    public class ArticleCategoryReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int ArticleCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}