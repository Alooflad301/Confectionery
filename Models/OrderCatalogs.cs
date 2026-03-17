using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confectionery.Models
{
    public class OrderCatalog
    {
        [Key]
        public int Id_OrderCatalog { get; set; }

        public int Id_Order { get; set; }
        [ForeignKey("Id_Order")]
        public Order Order { get; set; }

        public int Id_Catalog { get; set; }
        [ForeignKey("Id_Catalog")]
        public Catalog Catalog { get; set; }

        public int Quantity { get; set; }
    }

}
