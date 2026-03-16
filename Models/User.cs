using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confectionery.Models
{
    public class User
    {
        [Key]
        public int Id_User { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        public int Id_Role { get; set; } = 2;

        [ForeignKey("Id_Role")]
        public Role? Role { get; set; }

        [Required]
        [StringLength(50)]
        public string? Password { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string? Email { get; set; }

        [NotMapped]
        public bool RememberMe { get; set; }

        public ICollection<Order>? Orders { get; set; }
        public ICollection<Basket>? Baskets { get; set; }
    }
}
