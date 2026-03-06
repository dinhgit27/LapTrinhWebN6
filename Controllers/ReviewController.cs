using Microsoft.AspNetCore.Mvc;
using YourProjectName.Models;

namespace YourProjectName.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
       
        private static List<ProductReview> _reviews = new List<ProductReview>();

      
        [HttpGet("product/{productId}")]
        public IActionResult GetProductReviews(int productId)
        {
            var productReviews = _reviews.Where(r => r.ProductId == productId)
                                         .OrderByDescending(r => r.CreatedAt)
                                         .ToList();

            if (!productReviews.Any()) return Ok(new { Message = "Chưa có đánh giá nào." });

           
            double avgRating = productReviews.Average(r => r.Rating);

            return Ok(new {
                ProductId = productId,
                AverageRating = Math.Round(avgRating, 1),
                TotalReviews = productReviews.Count,
                Data = productReviews
            });
        }

       
        [HttpPost("submit")]
        public IActionResult SubmitReview([FromBody] ReviewRequest request)
        {
            // Validation cơ bản
            if (request.Rating < 1 || request.Rating > 5)
                return BadRequest("Số sao đánh giá phải từ 1 đến 5.");

            if (string.IsNullOrEmpty(request.Comment))
                return BadRequest("Nội dung đánh giá không được để trống.");

            var newReview = new ProductReview
            {
                Id = _reviews.Count + 1,
                ProductId = request.ProductId,
                UserName = request.UserName,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.Now
            };

            _reviews.Add(newReview);

            return Ok(new { Message = "Cảm ơn bạn đã đánh giá!", Review = newReview });
        }
    }
}