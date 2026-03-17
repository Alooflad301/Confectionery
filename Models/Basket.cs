using Confectionery.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Basket
{
    [Key]
    public int Id_Basket { get; set; }

    [Required]
    public int Id_User { get; set; }

    [ForeignKey("Id_User")]
    public User User { get; set; } = null!;

    // Итоговая сумма корзины
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total_Price { get; set; }

    // ✅ СТАТУС ЗАКАЗА (УЖЕ ЕСТЬ)
    public bool IsOrdered { get; set; } = false;

    // ✅ ДАТА СОЗДАНИЯ (УЖЕ ЕСТЬ)  
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // ✅ ТОВАРЫ В КОРЗИНЕ (УЖЕ ЕСТЬ)
    public ICollection<BasketCatalog> BasketCatalogs { get; set; } = new List<BasketCatalog>();

    // ✅ НОВОЕ: Связь с заказом (nullable)
    public int? Id_Order { get; set; }

    [ForeignKey("Id_Order")]
    public Order? Order { get; set; }

    // ✅ ВЫЧИСЛЯЕМЫЕ СВОЙСТВА (НЕ В БД)
    [NotMapped]
    public int TotalItems => BasketCatalogs.Sum(bc => bc.Quantity);

    [NotMapped]
    public bool IsEmpty => !BasketCatalogs.Any();

    // ✅ МЕТОДЫ ДЛЯ УДОБСТВА
    public void UpdateTotalPrice()
    {
        Total_Price = BasketCatalogs.Sum(bc => bc.Quantity * bc.Catalog.Price);
    }

    public BasketCatalog? GetItem(int catalogId)
    {
        return BasketCatalogs.FirstOrDefault(bc => bc.Id_Catalog == catalogId);
    }
}

