using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class UserAddress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactName { get; set; }

        [Required]
        [StringLength(15)]
        public string ContactPhone { get; set; }

        [Required]
        [StringLength(255)]
        public string AddressLine { get; set; }

        [Required]
        [StringLength(50)]
        public string Province { get; set; }

        [Required]
        [StringLength(50)]
        public string District { get; set; }

        [Required]
        [StringLength(50)]
        public string Ward { get; set; }

        public bool? IsDefault { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}