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

        public int? PreviousStatus { get; set; }

        [Required]
        public int NewStatus { get; set; }

        [StringLength(255)]
        public string Note { get; set; }

        [StringLength(100)]
        public string UpdatedBy { get; set; }

        public DateTime? Timestamp { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
    }
}
}