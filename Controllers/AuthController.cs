using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FashionEcommerce.Models;
using FashionEcommerce.Data;
using FashionEcommerce.Models.DTOs; // Đảm bảo DTOs nằm trong namespace này
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Google.Apis.Auth;

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // --- I. ĐĂNG KÝ (REGISTER) ---
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // 1. Kiểm tra Email tồn tại
            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("Email đã được sử dụng bởi một tài khoản khác.");

            // 2. Tạo đối tượng User mới
            var user = new User
            {
                Email = dto.Email,
                FullName = dto.FullName,
                Username = dto.Email.Split('@')[0], // Tạo username tạm thời từ Email
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // Dùng BCrypt đồng bộ
                Role = "Customer",
                CreatedAt = DateTime.UtcNow,
                IsLocked = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 3. Tạo Token và trả về thông tin cho Frontend
            var token = GenerateJwt(user);
            return Ok(new 
            { 
                token = token,
                userId = user.Id,
                fullName = user.FullName
            });
        }

        // --- II. ĐĂNG NHẬP (LOGIN) ---
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // 1. Tìm User theo Email
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            // 2. Kiểm tra sự tồn tại và mật khẩu
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Email hoặc mật khẩu không chính xác.");

            // 3. Kiểm tra tài khoản có bị khóa không
            if (user.IsLocked == true)
                return StatusCode(403, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ hỗ trợ.");

            // 4. Tạo Token và trả về (Khớp với yêu cầu của login.html)
            var token = GenerateJwt(user);

            return Ok(new 
            { 
                token = token,
                userId = user.Id,
                fullName = user.FullName,
                role = user.Role
            });
        }

        // --- III. ĐĂNG NHẬP GOOGLE (GOOGLE LOGIN) ---
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginDto dto)
        {
            try 
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);

                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.Email == payload.Email);

                // Nếu chưa có tài khoản thì tạo mới
                if (user == null)
                {
                    user = new User
                    {
                        Email = payload.Email,
                        FullName = payload.Name,
                        Username = payload.Email.Split('@')[0],
                        GoogleId = payload.Subject,
                        Role = "Customer",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                var token = GenerateJwt(user);
                return Ok(new { 
                    token = token, 
                    userId = user.Id, 
                    fullName = user.FullName 
                });
            }
            catch (Exception)
            {
                return BadRequest("Xác thực Google thất bại.");
            }
        }

        // --- IV. TẠO MÃ JWT (TOKEN GENERATION) ---
        private string GenerateJwt(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName ?? "")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}