using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserAddresses)
                .ToListAsync();

            return users.Select(u => new UserReadDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                FullName = u.FullName
            }).ToList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserAddresses)
                .Include(u => u.Orders)
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return new UserReadDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FullName = user.FullName
            };
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserReadDto>> PostUser(UserCreateDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var read = new UserReadDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FullName = user.FullName
            };

            return CreatedAtAction("GetUser", new { id = user.Id }, read);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserReadDto userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Update safe fields only
            user.Email = userDto.Email;
            user.Username = userDto.Username;
            user.FullName = userDto.FullName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
    }
}