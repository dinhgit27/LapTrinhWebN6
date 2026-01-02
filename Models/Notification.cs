using System;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public bool? IsRead { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}