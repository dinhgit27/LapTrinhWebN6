using System;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class OrderStatusHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.Now;

        public int? ChangedBy { get; set; } // User Id

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual User ChangedByUser { get; set; }
    }
}