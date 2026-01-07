using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models.DTOs
{
    public class UserCreateDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
