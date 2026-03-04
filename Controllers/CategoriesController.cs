using FashionEcommerce.Data;
using FashionEcommerce.DTOs;
using FashionEcommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.API.Controllers
{
    /// <summary>
    /// API Controller để quản lý danh mục sản phẩm (CRUD Operations)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục
        /// GET: api/Categories
        /// </summary>
        /// <param name="onlyActive">Chỉ lấy danh mục đang hoạt động</param>
        /// <returns>Danh sách danh mục</returns>
        /// <response code="200">Trả về danh sách danh mục</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetCategories([FromQuery] bool? onlyActive = null)
        {
            try
            {
                var query = _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .AsQueryable();

                // Lọc theo trạng thái hoạt động nếu được chỉ định
                if (onlyActive.HasValue && onlyActive.Value)
                {
                    query = query.Where(c => c.IsActive == true);
                }

                var categories = await query.ToListAsync();

                // Map sang DTO
                var categoryDtos = categories.Select(c => new CategoryReadDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    ParentId = c.ParentId,
                    ParentName = c.Parent?.Name,
                    IsActive = c.IsActive ?? false,
                    ProductCount = c.Products.Count,
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Lỗi khi lấy danh sách danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết một danh mục theo ID
        /// GET: api/Categories/5
        /// </summary>
        /// <param name="id">ID danh mục</param>
        /// <returns>Thông tin chi tiết danh mục</returns>
        /// <response code="200">Danh mục được tìm thấy</response>
        /// <response code="404">Danh mục không tồn tại</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                {
                    return NotFound(new { message = $"Danh mục với ID {id} không tồn tại" });
                }

                var categoryDto = new CategoryReadDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    ParentId = category.ParentId,
                    ParentName = category.Parent?.Name,
                    IsActive = category.IsActive ?? false,
                    ProductCount = category.Products.Count,
                    ChildrenCount = category.Children.Count
                };

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Lỗi khi lấy thông tin danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh mục theo Slug
        /// GET: api/Categories/slug/{slug}
        /// </summary>
        /// <param name="slug">Slug của danh mục</param>
        /// <returns>Thông tin danh mục</returns>
        [HttpGet("slug/{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryReadDto>> GetCategoryBySlug(string slug)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Slug == slug.ToLower());

                if (category == null)
                {
                    return NotFound(new { message = $"Danh mục với slug '{slug}' không tồn tại" });
                }

                var categoryDto = new CategoryReadDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    ParentId = category.ParentId,
                    ParentName = category.Parent?.Name,
                    IsActive = category.IsActive ?? false,
                    ProductCount = category.Products.Count,
                    ChildrenCount = category.Children.Count
                };

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Lỗi khi lấy danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh mục con của một danh mục cha
        /// GET: api/Categories/5/children
        /// </summary>
        /// <param name="parentId">ID danh mục cha</param>
        /// <returns>Danh sách danh mục con</returns>
        [HttpGet("{parentId}/children")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetChildCategories(int parentId)
        {
            try
            {
                // Kiểm tra danh mục cha có tồn tại
                var parent = await _context.Categories.FindAsync(parentId);
                if (parent == null)
                {
                    return NotFound(new { message = $"Danh mục cha với ID {parentId} không tồn tại" });
                }

                var children = await _context.Categories
                    .Where(c => c.ParentId == parentId)
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .ToListAsync();

                var childDtos = children.Select(c => new CategoryReadDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    ParentId = c.ParentId,
                    ParentName = c.Parent?.Name,
                    IsActive = c.IsActive ?? false,
                    ProductCount = c.Products.Count,
                    ChildrenCount = c.Children.Count
                }).ToList();

                return Ok(childDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Lỗi khi lấy danh mục con", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo danh mục mới
        /// POST: api/Categories
        /// </summary>
        /// <param name="createDto">Dữ liệu tạo danh mục mới</param>
        /// <returns>Danh mục vừa tạo</returns>
        /// <response code="201">Danh mục được tạo thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="409">Slug đã tồn tại</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CategoryReadDto>> PostCategory(CategoryCreateDto createDto)
        {
            try
            {
                // Validate ModelState
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Kiểm tra slug đã tồn tại chưa
                var existingSlug = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Slug == createDto.Slug.ToLower());

                if (existingSlug != null)
                {
                    return Conflict(new { message = $"Slug '{createDto.Slug}' đã tồn tại" });
                }

                // Nếu có ParentId, kiểm tra danh mục cha có tồn tại không
                if (createDto.ParentId.HasValue)
                {
                    var parentCategory = await _context.Categories.FindAsync(createDto.ParentId);
                    if (parentCategory == null)
                    {
                        return BadRequest(new { message = $"Danh mục cha với ID {createDto.ParentId} không tồn tại" });
                    }
                }

                // Tạo danh mục mới
                var category = new Category
                {
                    Name = createDto.Name.Trim(),
                    Slug = createDto.Slug.ToLower().Trim(),
                    ParentId = createDto.ParentId,
                    IsActive = createDto.IsActive
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Reload để có navigation properties
                await _context.Entry(category).Reference(c => c.Parent).LoadAsync();
                await _context.Entry(category).Collection(c => c.Children).LoadAsync();
                await _context.Entry(category).Collection(c => c.Products).LoadAsync();

                var categoryDto = new CategoryReadDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    ParentId = category.ParentId,
                    ParentName = category.Parent?.Name,
                    IsActive = category.IsActive ?? false,
                    ProductCount = category.Products.Count,
                    ChildrenCount = category.Children.Count
                };

                return CreatedAtAction("GetCategory", new { id = category.Id }, categoryDto);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Lỗi cơ sở dữ liệu khi tạo danh mục", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Lỗi khi tạo danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin danh mục
        /// PUT: api/Categories/5
        /// </summary>
        /// <param name="id">ID danh mục cần cập nhật</param>
        /// <param name="updateDto">Dữ liệu cập nhật</param>
        /// <returns>Danh mục đã cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="404">Danh mục không tồn tại</response>
        /// <response code="409">Slug đã tồn tại</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CategoryReadDto>> PutCategory(int id, CategoryUpdateDto updateDto)
        {
            try
            {
                // Validate ModelState
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Tìm danh mục hiện tại
                var category = await _context.Categories
                    .Include(c => c.Parent)
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound(new { message = $"Danh mục với ID {id} không tồn tại" });
                }

                // Kiểm tra slug nếu slug được thay đổi
                if (category.Slug != updateDto.Slug.ToLower())
                {
                    var existingSlug = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Slug == updateDto.Slug.ToLower() && c.Id != id);

                    if (existingSlug != null)
                    {
                        return Conflict(new { message = $"Slug '{updateDto.Slug}' đã được sử dụng" });
                    }
                }

                // Nếu ParentId thay đổi, kiểm tra danh mục cha
                if (category.ParentId != updateDto.ParentId && updateDto.ParentId.HasValue)
                {
                    // Kiểm tra self-reference (danh mục không thể là cha của chính nó)
                    if (updateDto.ParentId == id)
                    {
                        return BadRequest(new { message = "Danh mục không thể là cha của chính nó" });
                    }

                    var parentCategory = await _context.Categories.FindAsync(updateDto.ParentId);
                    if (parentCategory == null)
                    {
                        return BadRequest(new { message = $"Danh mục cha với ID {updateDto.ParentId} không tồn tại" });
                    }
                }

                // Cập nhật dữ liệu
                category.Name = updateDto.Name.Trim();
                category.Slug = updateDto.Slug.ToLower().Trim();
                category.ParentId = updateDto.ParentId;
                category.IsActive = updateDto.IsActive;

                _context.Entry(category).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(id))
                    {
                        return NotFound(new { message = $"Danh mục với ID {id} không tồn tại" });
                    }
                    throw;
                }

                var categoryDto = new CategoryReadDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    ParentId = category.ParentId,
                    ParentName = category.Parent?.Name,
                    IsActive = category.IsActive ?? false,
                    ProductCount = category.Products.Count,
                    ChildrenCount = category.Children.Count
                };

                return Ok(new { message = "Cập nhật danh mục thành công", data = categoryDto });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Lỗi cơ sở dữ liệu khi cập nhật danh mục", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Lỗi khi cập nhật danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa danh mục
        /// DELETE: api/Categories/5
        /// </summary>
        /// <param name="id">ID danh mục cần xóa</param>
        /// <returns>Thông báo kết quả</returns>
        /// <response code="200">Xóa thành công</response>
        /// <response code="404">Danh mục không tồn tại</response>
        /// <response code="409">Không thể xóa do danh mục còn chứa dữ liệu</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Children)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound(new { message = $"Danh mục với ID {id} không tồn tại" });
                }

                // Kiểm tra nếu danh mục còn sản phẩm
                if (category.Products.Any())
                {
                    return Conflict(new 
                    { 
                        message = "Không thể xóa danh mục vì còn chứa sản phẩm",
                        productCount = category.Products.Count
                    });
                }

                // Kiểm tra nếu danh mục còn danh mục con
                if (category.Children.Any())
                {
                    return Conflict(new 
                    { 
                        message = "Không thể xóa danh mục vì còn chứa danh mục con",
                        childrenCount = category.Children.Count
                    });
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Xóa danh mục ID {id} thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Lỗi khi xóa danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra danh mục có tồn tại không
        /// </summary>
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
