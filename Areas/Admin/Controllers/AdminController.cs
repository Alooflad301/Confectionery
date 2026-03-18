using Confectionery.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Area("Admin")]
[Authorize(Roles = "Администратор")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAdminData()
    {
        var data = new
        {
            usersCount = _context.Users.Count(),
            ordersCount = _context.Orders.Count(),
            productsCount = _context.Catalog.Count()
        };
        return Json(data);
    }
}
