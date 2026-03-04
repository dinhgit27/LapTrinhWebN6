using FashionEcommerce.Data;
using FashionEcommerce.Models;
using FashionEcommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AddressesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressReadDto>>> GetAddresses()
        {
            var list = await _context.UserAddresses.ToListAsync();
            return list.Select(a => new AddressReadDto
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
            }).ToList();
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressReadDto>> GetAddress(int id)
        {
            var a = await _context.UserAddresses.FindAsync(id);
            if (a == null) return NotFound();
            return new AddressReadDto
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
            };
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<ActionResult<AddressReadDto>> PostAddress(AddressCreateDto dto)
        {
            var address = new UserAddress
            {
                UserId = dto.UserId,
                ContactName = dto.ContactName,
                ContactPhone = dto.ContactPhone,
                AddressLine = dto.AddressLine,
                Province = dto.Province,
                District = dto.District,
                Ward = dto.Ward,
                IsDefault = dto.IsDefault
            };
            _context.UserAddresses.Add(address);
            await _context.SaveChangesAsync();

            var read = new AddressReadDto
            {
                Id = address.Id,
                UserId = address.UserId,
                ContactName = address.ContactName,
                ContactPhone = address.ContactPhone,
                AddressLine = address.AddressLine,
                Province = address.Province,
                District = address.District,
                Ward = address.Ward,
                IsDefault = address.IsDefault
            };

            return CreatedAtAction("GetAddress", new { id = address.Id }, read);
        }

        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, AddressUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();
            var address = await _context.UserAddresses.FindAsync(id);
            if (address == null) return NotFound();

            address.ContactName = dto.ContactName;
            address.ContactPhone = dto.ContactPhone;
            address.AddressLine = dto.AddressLine;
            address.Province = dto.Province;
            address.District = dto.District;
            address.Ward = dto.Ward;
            address.IsDefault = dto.IsDefault;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserAddresses.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.UserAddresses.FindAsync(id);
            if (address == null) return NotFound();
            _context.UserAddresses.Remove(address);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}