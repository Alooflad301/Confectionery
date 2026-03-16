using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confectionery.Models
{
    public class Category
    {
        [Key]
        public int Id_Category { get; set; }

        [Required(ErrorMessage = "Название категории обязательно")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название от 2 до 50 символов")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        // Навигационное свойство - связь с товарами
        public ICollection<Catalog>? Catalogs { get; set; }

        // Вычисляемое свойство (не сохраняется в БД)
        [NotMapped]
        public int ProductsCount => Catalogs?.Count ?? 0;
    }
}
