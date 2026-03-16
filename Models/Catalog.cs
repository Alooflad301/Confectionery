using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Confectionery.Models
{
    public class Catalog
    {
        [Key]
        public int Id_Catalog { get; set; }

        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Название должно быть от 3 до 100 символов")]
        public string Product { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Описание не может превышать 500 символов")]
        public string? Description { get; set; }

        // Фото товара
        public byte[]? Photo { get; set; }

        [StringLength(20)]
        public string? PhotoContentType { get; set; } // image/jpeg, image/png

        [StringLength(100)]
        public string? PhotoFileName { get; set; }

        public long? PhotoSize { get; set; }

        // Цена
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        // Связь с категорией
        [Required]
        public int Id_Ctegory { get; set; }

        [ForeignKey("Id_Ctegory")]
        public Category? Category { get; set; }

        // Навигационные свойства (убрали OrderCatalogs)
        public ICollection<BasketCatalog>? BasketCatalogs { get; set; }

        // Вычисляемые свойства для фронтенда
        [NotMapped]
        public string? PhotoBase64 => Photo != null
            ? $"data:{PhotoContentType};base64,{Convert.ToBase64String(Photo)}"
            : null;

        [NotMapped]
        public bool HasPhoto => Photo != null && Photo.Length > 0;

        [NotMapped]
        public string PriceDisplay => Price.ToString("C"); // Форматированная цена

        // Конструктор по умолчанию
        public Catalog()
        {
            Price = 0;
        }
    }
}
