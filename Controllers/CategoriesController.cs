using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- HELPER METHODS ---

        /// <summary>
        /// Map Category entity to CategoryReadDto
        /// </summary>
        private CategoryReadDto MapToCategoryReadDto(Category category)
        {
            return new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ParentId = category.ParentId > 0 ? category.ParentId : null,
                IsActive = category.IsActive,
                Parent = category.Parent != null ? MapToCategoryReadDto(category.Parent) : null,
                Children = category.Children != null ? category.Children.Select(MapToCategoryReadDto).ToList() : new List<CategoryReadDto>(),
                ProductCount = category.Products != null ? category.Products.Count : 0
            };
        }

        // --- CRUD OPERATIONS ---

        /// <summary>
        /// GET: api/categories
        /// Lấy danh sách tất cả danh mục (bao gồm cả danh mục tổ chức theo cấp bậc)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .Where(c => c.ParentId == 0)  // Chỉ lấy danh mục gốc (không có parent)
                    .ToListAsync();

                return Ok(categories.Select(MapToCategoryReadDto).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/categories/all
        /// Lấy tất cả danh mục (bao gồm cả danh mục con)
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAllCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .ToListAsync();

                // Chỉ trả về danh mục gốc, các danh mục con sẽ được bao gồm trong thuộc tính Children
                var rootCategories = categories
                    .Where(c => c.ParentId == 0)
                    .Select(MapToCategoryReadDto)
                    .ToList();

                return Ok(rootCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy tất cả danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/categories/{id}
        /// Lấy thông tin chi tiết một danh mục cụ thể
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryReadDto>> GetCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return NotFound(new { message = $"Không tìm thấy danh mục với ID {id}" });

                return Ok(MapToCategoryReadDto(category));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/categories/by-slug/{slug}
        /// Lấy danh mục theo Slug
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        public async Task<ActionResult<CategoryReadDto>> GetCategoryBySlug(string slug)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(slug))
                    return BadRequest(new { message = "Slug không được để trống" });

                var category = await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Slug == slug.ToLower());

                if (category == null)
                    return NotFound(new { message = $"Không tìm thấy danh mục với slug '{slug}'" });

                return Ok(MapToCategoryReadDto(category));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh mục theo slug", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/categories
        /// Tạo một danh mục mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoryReadDto>> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(new { message = "Tên danh mục không được để trống" });

                if (string.IsNullOrWhiteSpace(dto.Slug))
                    return BadRequest(new { message = "Slug không được để trống" });

                // Kiểm tra slug đã tồn tại
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Slug == dto.Slug.ToLower());

                if (existingCategory != null)
                    return BadRequest(new { message = $"Slug '{dto.Slug}' đã tồn tại" });

                // Nếu có ParentId, kiểm tra parent category tồn tại
                if (dto.ParentId.HasValue && dto.ParentId > 0)
                {
                    var parentCategory = await _context.Categories.FindAsync(dto.ParentId);
                    if (parentCategory == null)
                        return BadRequest(new { message = $"Danh mục cha với ID {dto.ParentId} không tồn tại" });

                    // Kiểm tra không tạo vòng lặp (circular reference)
                    if (await HasCircularReference(dto.ParentId.Value, 0))
                        return BadRequest(new { message = "Không thể tạo danh mục con của chính nó" });
                }

                var category = new Category
                {
                    Name = dto.Name.Trim(),
                    Slug = dto.Slug.ToLower().Trim(),
                    ParentId = dto.ParentId ?? 0,
                    IsActive = dto.IsActive
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Reload để lấy kèm Parent và Children data
                var createdCategory = await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == category.Id);

                return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, MapToCategoryReadDto(createdCategory));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi database khi tạo danh mục", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/categories/{id}
        /// Cập nhật thông tin danh mục
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
        {
            try
            {
                // Validate input
                if (id != dto.Id)
                    return BadRequest(new { message = "ID trong URL không khớp với ID trong body" });

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(new { message = "Tên danh mục không được để trống" });

                if (string.IsNullOrWhiteSpace(dto.Slug))
                    return BadRequest(new { message = "Slug không được để trống" });

                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    return NotFound(new { message = $"Không tìm thấy danh mục với ID {id}" });

                // Kiểm tra slug có đã được sử dụng bởi danh mục khác
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Slug == dto.Slug.ToLower() && c.Id != id);

                if (existingCategory != null)
                    return BadRequest(new { message = $"Slug '{dto.Slug}' đã tồn tại" });

                // Nếu ParentId thay đổi, kiểm tra hợp lệ
                if (dto.ParentId != category.ParentId && dto.ParentId.HasValue && dto.ParentId > 0)
                {
                    var parentCategory = await _context.Categories.FindAsync(dto.ParentId);
                    if (parentCategory == null)
                        return BadRequest(new { message = $"Danh mục cha với ID {dto.ParentId} không tồn tại" });

                    // Kiểm tra không tạo vòng lặp
                    if (await HasCircularReference(dto.ParentId.Value, id))
                        return BadRequest(new { message = "Không thể đặt danh mục con làm cha của danh mục cha" });
                }

                category.Name = dto.Name.Trim();
                category.Slug = dto.Slug.ToLower().Trim();
                category.ParentId = dto.ParentId ?? 0;
                category.IsActive = dto.IsActive;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                // Reload để trả về dữ liệu thu được cập nhật
                var updatedCategory = await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                return Ok(new
                {
                    message = "Cập nhật danh mục thành công",
                    data = MapToCategoryReadDto(updatedCategory)
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi database khi cập nhật danh mục", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/categories/{id}
        /// Xóa một danh mục (chỉ có thể xóa nếu không có sản phẩm)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .Include(c => c.Children)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return NotFound(new { message = $"Không tìm thấy danh mục với ID {id}" });

                // Kiểm tra có sản phẩm trong danh mục
                if (category.Products.Any())
                    return BadRequest(new { message = "Không thể xóa danh mục vì còn có sản phẩm trong danh mục này" });

                // Kiểm tra có danh mục con
                if (category.Children.Any())
                    return BadRequest(new { message = "Không thể xóa danh mục vì còn có danh mục con" });

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa danh mục thành công" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi database khi xóa danh mục", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// PATCH: api/categories/{id}/toggle-active
        /// Bật/tắt trạng thái hoạt động của danh mục
        /// </summary>
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return NotFound(new { message = $"Không tìm thấy danh mục với ID {id}" });

                category.IsActive = !category.IsActive;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Danh mục đã {(category.IsActive ? "kích hoạt" : "vô hiệu hóa")}",
                    data = MapToCategoryReadDto(category)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật trạng thái danh mục", error = ex.Message });
            }
        }

        // --- HELPER METHODS ---

        /// <summary>
        /// Kiểm tra có vòng lặp phân cấp (circular reference)
        /// Tránh tình huống: A -> B -> C -> A
        /// </summary>
        private async Task<bool> HasCircularReference(int parentId, int categoryId)
        {
            var visited = new HashSet<int>();
            int currentId = parentId;

            while (currentId > 0)
            {
                if (currentId == categoryId)
                    return true;  // Phát hiện vòng lặp

                if (visited.Contains(currentId))
                    return true;  // Phát hiện vòng lặp

                visited.Add(currentId);

                var parent = await _context.Categories.FindAsync(currentId);
                if (parent == null || parent.ParentId == 0)
                    break;

                currentId = parent.ParentId;
            }

            return false;
        }
    }
}
