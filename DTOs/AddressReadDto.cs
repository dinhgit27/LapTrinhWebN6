namespace FashionEcommerce.Models.DTOs
{
    public class AddressReadDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ContactName { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
        public string AddressLine { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public bool? IsDefault { get; set; }
    }
}