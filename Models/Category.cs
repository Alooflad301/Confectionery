using System.ComponentModel.DataAnnotations;

namespace Confectionery.Models
{
    public class Category
    {
        [Key]
        public int Id_Category { get; set; }

        [Required]
        [StringLength(50)]
        public int Name { get; set; }

        public ICollection<Catalog>? Catalogs { get; set; }
    }
}
