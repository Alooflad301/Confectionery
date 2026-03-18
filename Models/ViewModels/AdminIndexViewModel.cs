namespace Confectionery.Models.ViewModels
{
    public class AdminIndexViewModel
    {
        public int UsersCount { get; set; }
        public int OrdersCount { get; set; }
        public int ProductsCount { get; set; }
        public int CategoriesCount { get; set; }
        public List<object> RecentOrders { get; set; } = new();
    }
}