namespace YourProjectName.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; } // Từ 1 đến 5 sao
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class ReviewRequest
    {
        public int ProductId { get; set; }
        public string UserName { get; set; } = "Khách hàng ẩn danh";
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}