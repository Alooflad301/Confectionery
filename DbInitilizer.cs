using Microsoft.EntityFrameworkCore;
using Confectionery.Data;
using Confectionery.Models;

namespace Confectionery.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            // 🔥 РОЛИ (2 шт)
            if (!context.Roles.Any())
            {
                var roles = new Role[]
                {
            new Role {Name = "Пользователь" },
            new Role {Name = "Администратор" }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            // 🔥 КАТЕГОРИИ (6 шт)
            if (!context.Categories.Any())
            {
                var categories = new Category[]
                {
            new Category { Name = "Торты", Description = "На день рождения, свадьбу, праздники" },
            new Category { Name = "Пирожные", Description = "Эклеры, макаронс, профитроли" },
            new Category { Name = "Пироги", Description = "Мясные, рыбные, овощные, сладкие" },
            new Category { Name = "Кексы", Description = "Маффины, капкейки, кексики" },
            new Category { Name = "Десерты", Description = "Чизкейки, пудинги, мороженое" },
            new Category { Name = "Конфеты", Description = "Шоколадные, леденцы, мармелад" }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // 🔥 ТОВАРЫ (15+ штук 🍰🧁🍬)
            if (!context.Catalog.Any())
            {
                var catalogs = new Catalog[]
                {
            // 🎂 ТОРТЫ (5 шт)
            new Catalog { Product = "Торт Медовик", Id_Ctegory = 1, Description = "Слоеный медовик со сметанным кремом", Price = 2500m, PhotoPath = "medovik.jpg" },
            new Catalog { Product = "Чизкейк Нью-Йорк", Id_Ctegory = 1, Description = "Классический NY чизкейк с ягодами", Price = 2800m, PhotoPath = "cheesecake.jpg" },
            new Catalog { Product = "Торт Наполеон", Id_Ctegory = 1, Description = "Слоеный торт с заварным кремом", Price = 2200m, PhotoPath = "napoleon.jpg" },
            new Catalog { Product = "Красный бархат", Id_Ctegory = 1, Description = "Нежный шоколадный бисквит с крем-чиз", Price = 3200m, PhotoPath = "velvet.jpg" },
            new Catalog { Product = "Торт Зебра", Id_Ctegory = 1, Description = "Шоколадно-ванильный мраморный торт", Price = 1900m, PhotoPath = "zebra.jpg" },

            // 🧁 ПИРОЖНЫЕ (4 шт)
            new Catalog { Product = "Эклер шоколадный", Id_Ctegory = 2, Description = "Хрустящий профитроль с шоколадным кремом", Price = 150m, PhotoPath = "ecler.jpg" },
            new Catalog { Product = "Макаронс (6 шт)", Id_Ctegory = 2, Description = "Французские миндальные пирожные", Price = 450m, PhotoPath = "macarons.jpg" },
            new Catalog { Product = "Профитроли", Id_Ctegory = 2, Description = "С заварным кремом и шоколадом", Price = 320m, PhotoPath = "profiterol.jpg" },
            new Catalog { Product = "Тирамису мини", Id_Ctegory = 2, Description = "Итальянский десерт с маскарпоне", Price = 280m, PhotoPath = "tiramisu.jpg" },

            // 🥧 ПИРОГИ (2 шт)
            new Catalog { Product = "Пирог с вишней", Id_Ctegory = 3, Description = "Дрожжевое тесто с вишневой начинкой", Price = 850m, PhotoPath = "cherry-pie.jpg" },
            new Catalog { Product = "Мясной пирог", Id_Ctegory = 3, Description = "С говядиной и луком", Price = 950m, PhotoPath = "meat-pie.jpg" },

            // 🧁 КЕКСЫ (2 шт)
            new Catalog { Product = "Капкейк ванильный", Id_Ctegory = 4, Description = "С масляным кремом", Price = 120m, PhotoPath = "cupcake.jpg" },
            new Catalog { Product = "Маффин шоколадный", Id_Ctegory = 4, Description = "С шоколадными каплями", Price = 110m, PhotoPath = "muffin.jpg" },

            // 🍰 ДЕСЕРТЫ (2 шт)
            new Catalog { Product = "Панна котта", Id_Ctegory = 5, Description = "Итальянский десерт с ягодным соусом", Price = 350m, PhotoPath = "panna-cotta.jpg" },
            new Catalog { Product = "Трюфели (5 шт)", Id_Ctegory = 6, Description = "Шоколадные трюфели ручной работы", Price = 420m, PhotoPath = "truffles.jpg" }
                };
                context.Catalog.AddRange(catalogs);
                context.SaveChanges();
            }

            if (!context.StatusOrders.Any())
            {
                var statusOrders = new StatusOrder[]
                {
            new StatusOrder {Name = "Новый"},        // Синий
            new StatusOrder {Name = "Подтвержден"},  // Зеленый
            new StatusOrder {Name = "Готовится"},  // Желтый
            new StatusOrder {Name = "В доставке"}, // Голубой
            new StatusOrder {Name = "Выполнен"},      // Зеленый
            new StatusOrder {Name = "Отменен"}        // Красный
                };
                context.StatusOrders.AddRange(statusOrders);
                context.SaveChanges();
            }

            // 🔥 АДМИН + ПОЛЬЗОВАТЕЛЬ
            if (!context.Users.Any(u => u.Login == "admin"))
            {
                context.Users.Add(new User
                {
                    Login = "admin",
                    Name = "Администратор",
                    Password = "Admin123!",
                    Email = "admin@confectionery.ru",
                    Id_Role = 2
                });
                context.SaveChanges();
            }

            if (!context.Users.Any(u => u.Login == "user"))
            {
                context.Users.Add(new User
                {
                    Login = "user",
                    Name = "Тестовый Пользователь",
                    Password = "User123!",
                    Email = "user@example.com",
                    Id_Role = 1
                });
                context.SaveChanges();
            }
        }

    }
}
