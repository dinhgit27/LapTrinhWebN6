using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net; // Đảm bảo đã cài đặt thư viện BCrypt.Net-Next

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- CÁC PHƯƠNG THỨC QUẢN LÝ (CRUD) ---

        // GET: api/Users
        // Dùng cho Admin xem danh sách toàn bộ người dùng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserAddresses)
                .ToListAsync();

            return Ok(users.Select(u => MapToReadDto(u)).ToList());
        }

        // GET: api/Users/5
        // Lấy thông tin chi tiết của một người dùng cụ thể
        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserAddresses)
                .Include(u => u.Orders)
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound("Không tìm thấy người dùng.");

            return Ok(MapToReadDto(user));
        }

        // POST: api/Users
        // Dùng cho Admin tạo tài khoản thủ công hoặc Seed dữ liệu
        [HttpPost]
        public async Task<ActionResult<UserReadDto>> PostUser(UserCreateDto dto)
        {
            // Kiểm tra trùng email
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email đã tồn tại trong hệ thống.");

            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Role = "Customer", // Mặc định là khách hàng
                CreatedAt = DateTime.UtcNow,
                // ĐỒNG BỘ: Sử dụng BCrypt giống AuthController
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, MapToReadDto(user));
        }

        // PUT: api/Users/5
        // Cập nhật thông tin cá nhân (Profile)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserReadDto userDto)
        {
            if (id != userDto.Id) return BadRequest("ID không khớp.");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Chỉ cho phép cập nhật các trường an toàn
            user.Email = userDto.Email;
            user.Username = userDto.Username;
            user.FullName = userDto.FullName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --- CÁC HÀM HỖ TRỢ (HELPERS) ---

        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }

        // Hàm Mapping thủ công để Code gọn gàng hơn
        private static UserReadDto MapToReadDto(User u)
        {
            return new UserReadDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                FullName = u.FullName,
                Addresses = u.UserAddresses?.Select(a => new AddressReadDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    ContactName = a.ContactName,
                    ContactPhone = a.ContactPhone,
                    AddressLine = a.AddressLine,
                    Province = a.Province,
                    District = a.District,
                    Ward = a.Ward,
                    IsDefault = a.IsDefault
                }).ToList()
            };
        }
    }
}