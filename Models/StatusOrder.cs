using System.ComponentModel.DataAnnotations;

namespace Confectionery.Models
{
    public class StatusOrder
    {
        [Key]
        public int Id_StatusOrder { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
