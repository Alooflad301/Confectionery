using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confectionery.Models
{
    public class Order
    {
        [Key]
        public int Id_Order { get; set; }


        public int Id_User { get; set; }

        [ForeignKey("Id_User")]
        public User? User { get; set; }

        public int Id_StatusOrder { get; set; }

        [ForeignKey("Id_StatusOrder")]
        public StatusOrder? StatusOrder { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal Price { get; set; }

        public ICollection<OrderCatalog> OrderCatalogs { get; set; }

    }
}
