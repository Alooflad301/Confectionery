using System.ComponentModel.DataAnnotations;

namespace Confectionery.Models
{
    public class Role
    {
        [Key]
        public int Id_Role { get; set; }
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        public ICollection<User>? Users { get; set; }

    }
}
