using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confectionery.Models
{
    public class Basket
    {
        [Key]
        public int Id_Basket { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]

        public decimal Total_Price {  get; set; }

        public int Id_User { get; set; }

        [ForeignKey("Id_User")]

        public User? User { get; set; }

        public ICollection<BasketCatalog>? BasketCatalogs { get; set; }
    }
}
