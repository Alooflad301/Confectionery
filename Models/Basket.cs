using Confectionery.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Basket
{
    [Key]
    public int Id_Basket { get; set; }

    public int Id_User { get; set; }
    [ForeignKey("Id_User")]
    public User User { get; set; } = null!;

    // Итоговая сумма корзины
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total_Price { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public ICollection<BasketCatalog> BasketCatalogs { get; set; } = new List<BasketCatalog>();
}
