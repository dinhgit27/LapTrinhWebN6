using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.API.Controllers
{
    //api quan ly bai viet

    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- HELPER METHODS ---

        /// <summary>
        /// Map Article entity to ArticleReadDto
        /// </summary>
        private ArticleReadDto MapToArticleReadDto(Article article)
        {
            return new ArticleReadDto
            {
                Id = article.Id,
                Title = article.Title,
                Slug = article.Slug,
                Summary = article.Summary,
                Content = article.Content,
                Thumbnail = article.Thumbnail,
                CategoryId = article.CategoryId,
                CategoryName = article.Category?.Name,
                IsPublished = article.IsPublished ?? false,
                PublishedAt = article.PublishedAt,
                CreatedAt = article.CreatedAt,
                UpdatedAt = article.UpdatedAt
            };
        }

        // --- CRUD OPERATIONS FOR ARTICLES ---

        /// <summary>
        /// GET: api/Articles
        /// Lấy danh sách tất cả bài viết (chỉ admin)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleReadDto>>> GetArticles()
        {
            try
            {
                var articles = await _context.Articles
                    .Include(a => a.Category)
                    .ToListAsync();

                return Ok(articles.Select(MapToArticleReadDto).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/Articles/public
        /// Lấy danh sách bài viết công khai với phân trang
        /// </summary>
        [HttpGet("public")]
        public async Task<ActionResult> GetPublicArticles(
            [FromQuery] int? categoryId,
            [FromQuery] string? q,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Articles
                .Include(a => a.Category)
                .Where(a => a.IsPublished == true);

            if (categoryId.HasValue)
                query = query.Where(a => a.CategoryId == categoryId.Value);

            if (!string.IsNullOrEmpty(q))
            {
                var qLower = q.ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(qLower)
                    || (a.Summary != null && a.Summary.ToLower().Contains(qLower))
                    || a.Slug.ToLower().Contains(qLower));
            }

            query = query.OrderByDescending(a => a.PublishedAt);

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ArticlePublicDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Summary = a.Summary,
                    Thumbnail = a.Thumbnail,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category != null ? a.Category.Name : null,
                    PublishedAt = a.PublishedAt
                })
                .ToListAsync();

            return Ok(new { total, page, pageSize, items });
        }

        /// <summary>
        /// GET: api/Articles/{id}
        /// Lấy thông tin chi tiết một bài viết cụ thể
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleReadDto>> GetArticle(int id)
        {
            try
            {
                var article = await _context.Articles
                    .Include(a => a.Category)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (article == null)
                    return NotFound(new { message = $"Không tìm thấy bài viết với ID {id}" });

                return Ok(MapToArticleReadDto(article));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/Articles/by-slug/{slug}
        /// Lấy bài viết theo Slug
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        public async Task<ActionResult<ArticleReadDto>> GetArticleBySlug(string slug)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(slug))
                    return BadRequest(new { message = "Slug không được để trống" });

                var article = await _context.Articles
                    .Include(a => a.Category)
                    .FirstOrDefaultAsync(a => a.Slug == slug.ToLower());

                if (article == null)
                    return NotFound(new { message = $"Không tìm thấy bài viết với slug '{slug}'" });

                return Ok(MapToArticleReadDto(article));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy bài viết theo slug", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/Articles
        /// Tạo bài viết mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ArticleReadDto>> CreateArticle(ArticleCreateDto articleDto)
        {
            try
            {
                // Kiểm tra category tồn tại
                var category = await _context.ArticleCategories.FindAsync(articleDto.CategoryId);
                if (category == null)
                    return BadRequest(new { message = $"Không tìm thấy danh mục với ID {articleDto.CategoryId}" });

                // Kiểm tra slug duy nhất
                var existingArticle = await _context.Articles
                    .FirstOrDefaultAsync(a => a.Slug == articleDto.Slug.ToLower());
                if (existingArticle != null)
                    return BadRequest(new { message = "Slug đã tồn tại" });

                var article = new Article
                {
                    Title = articleDto.Title,
                    Slug = articleDto.Slug.ToLower(),
                    Summary = articleDto.Summary,
                    Content = articleDto.Content,
                    Thumbnail = articleDto.Thumbnail,
                    CategoryId = articleDto.CategoryId,
                    IsPublished = articleDto.IsPublished,
                    PublishedAt = articleDto.IsPublished ? articleDto.PublishedAt ?? DateTime.UtcNow : null,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Articles.Add(article);
                await _context.SaveChangesAsync();

                // Load category để trả về
                await _context.Entry(article).Reference(a => a.Category).LoadAsync();

                return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, MapToArticleReadDto(article));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/Articles/{id}
        /// Cập nhật bài viết
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, ArticleUpdateDto articleDto)
        {
            try
            {
                var article = await _context.Articles.FindAsync(id);
                if (article == null)
                    return NotFound(new { message = $"Không tìm thấy bài viết với ID {id}" });

                // Kiểm tra category tồn tại
                var category = await _context.ArticleCategories.FindAsync(articleDto.CategoryId);
                if (category == null)
                    return BadRequest(new { message = $"Không tìm thấy danh mục với ID {articleDto.CategoryId}" });

                // Kiểm tra slug duy nhất (trừ bài viết hiện tại)
                var existingArticle = await _context.Articles
                    .FirstOrDefaultAsync(a => a.Slug == articleDto.Slug.ToLower() && a.Id != id);
                if (existingArticle != null)
                    return BadRequest(new { message = "Slug đã tồn tại" });

                article.Title = articleDto.Title;
                article.Slug = articleDto.Slug.ToLower();
                article.Summary = articleDto.Summary;
                article.Content = articleDto.Content;
                article.Thumbnail = articleDto.Thumbnail;
                article.CategoryId = articleDto.CategoryId;
                article.IsPublished = articleDto.IsPublished;
                article.PublishedAt = articleDto.IsPublished ? articleDto.PublishedAt ?? DateTime.UtcNow : null;
                article.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/Articles/{id}
        /// Xóa bài viết
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            try
            {
                var article = await _context.Articles.FindAsync(id);
                if (article == null)
                    return NotFound(new { message = $"Không tìm thấy bài viết với ID {id}" });

                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa bài viết", error = ex.Message });
            }
        }
    }
}