namespace FashionEcommerce.Models.DTOs
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string? Username { get; set; }
        public string? FullName { get; set; }
    }
}
