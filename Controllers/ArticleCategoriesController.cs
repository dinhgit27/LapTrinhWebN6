using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//api quan ly danh muc

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArticleCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- HELPER METHODS ---

        /// <summary>
        /// Map ArticleCategory entity to ArticleCategoryReadDto
        /// </summary>
        private ArticleCategoryReadDto MapToArticleCategoryReadDto(ArticleCategory category)
        {
            return new ArticleCategoryReadDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ArticleCount = category.Articles?.Count ?? 0,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        // --- CRUD OPERATIONS FOR ARTICLE CATEGORIES ---

        /// <summary>
        /// GET: api/ArticleCategories
        /// Lấy danh sách tất cả danh mục bài viết
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleCategoryReadDto>>> GetArticleCategories()
        {
            try
            {
                var categories = await _context.ArticleCategories
                    .Include(ac => ac.Articles)
                    .ToListAsync();

                return Ok(categories.Select(MapToArticleCategoryReadDto).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách danh mục bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/ArticleCategories/{id}
        /// Lấy thông tin chi tiết một danh mục bài viết cụ thể
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleCategoryReadDto>> GetArticleCategory(int id)
        {
            try
            {
                var category = await _context.ArticleCategories
                    .Include(ac => ac.Articles)
                    .FirstOrDefaultAsync(ac => ac.Id == id);

                if (category == null)
                    return NotFound(new { message = $"Không tìm thấy danh mục bài viết với ID {id}" });

                return Ok(MapToArticleCategoryReadDto(category));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin danh mục bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/ArticleCategories/by-slug/{slug}
        /// Lấy danh mục bài viết theo Slug
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        public async Task<ActionResult<ArticleCategoryReadDto>> GetArticleCategoryBySlug(string slug)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(slug))
                    return BadRequest(new { message = "Slug không được để trống" });

                var category = await _context.ArticleCategories
                    .Include(ac => ac.Articles)
                    .FirstOrDefaultAsync(ac => ac.Slug == slug.ToLower());

                if (category == null)
                    return NotFound(new { message = $"Không tìm thấy danh mục bài viết với slug '{slug}'" });

                return Ok(MapToArticleCategoryReadDto(category));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh mục bài viết theo slug", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/ArticleCategories
        /// Tạo danh mục bài viết mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ArticleCategoryReadDto>> CreateArticleCategory(ArticleCategoryCreateDto categoryDto)
        {
            try
            {
                // Kiểm tra slug duy nhất
                var existingCategory = await _context.ArticleCategories
                    .FirstOrDefaultAsync(ac => ac.Slug == categoryDto.Slug.ToLower());
                if (existingCategory != null)
                    return BadRequest(new { message = "Slug đã tồn tại" });

                var category = new ArticleCategory
                {
                    Name = categoryDto.Name,
                    Slug = categoryDto.Slug.ToLower(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.ArticleCategories.Add(category);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetArticleCategory), new { id = category.Id }, MapToArticleCategoryReadDto(category));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo danh mục bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/ArticleCategories/{id}
        /// Cập nhật danh mục bài viết
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticleCategory(int id, ArticleCategoryUpdateDto categoryDto)
        {
            try
            {
                var category = await _context.ArticleCategories.FindAsync(id);
                if (category == null)
                    return NotFound(new { message = $"Không tìm thấy danh mục bài viết với ID {id}" });

                // Kiểm tra slug duy nhất (trừ danh mục hiện tại)
                var existingCategory = await _context.ArticleCategories
                    .FirstOrDefaultAsync(ac => ac.Slug == categoryDto.Slug.ToLower() && ac.Id != id);
                if (existingCategory != null)
                    return BadRequest(new { message = "Slug đã tồn tại" });

                category.Name = categoryDto.Name;
                category.Slug = categoryDto.Slug.ToLower();
                category.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật danh mục bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/ArticleCategories/{id}
        /// Xóa danh mục bài viết
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticleCategory(int id)
        {
            try
            {
                var category = await _context.ArticleCategories.FindAsync(id);
                if (category == null)
                    return NotFound(new { message = $"Không tìm thấy danh mục bài viết với ID {id}" });

                // Kiểm tra xem danh mục có bài viết không
                var hasArticles = await _context.Articles.AnyAsync(a => a.CategoryId == id);
                if (hasArticles)
                    return BadRequest(new { message = "Không thể xóa danh mục có chứa bài viết" });

                _context.ArticleCategories.Remove(category);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa danh mục bài viết", error = ex.Message });
            }
        }
    }
}