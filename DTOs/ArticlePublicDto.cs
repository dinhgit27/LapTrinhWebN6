using System;

namespace FashionEcommerce.DTOs
{
    //thong tin cong khai cho frontend, khong can thong tin chi tiet nhu content
    public class ArticlePublicDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string Thumbnail { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}