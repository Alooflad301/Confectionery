using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confectionery.Models
{
    public class Catalog
    {
        [Key]
        public int Id_Catalog { get; set; }

        [Required]
        [StringLength(50)]
        public string? Product { get; set; }

        public int Id_Ctegory { get; set; }
        [ForeignKey("Id_Ctegory")]
        public Category? Category { get; set; }

        public ICollection<BasketCatalog>? BasketCatalogs { get; set; }

    }
}
