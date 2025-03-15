using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteerManagment.Models
{
    public class Roles
    {
        [Key]
        [Required]
        public int RoleId { get; set; }
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
