using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confectionery.Models
{
    public class BasketCatalog
    {
        [Key]
        public int Id_BasketCatalog { get; set; }
        public int Id_Basket {  get; set; }

        [ForeignKey("Id_Basket")]
        public Basket? Basket { get; set; }

        public int Id_Catalog { get; set; }

        [ForeignKey("Id_Catalog")]
        public Catalog? Catalog { get; set; } 
    }
}
