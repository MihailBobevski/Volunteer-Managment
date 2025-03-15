using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace VolunteerManagment.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LName { get; set; }


        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public Roles Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<VolunteerEvent> VolunteerEvents { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<EventTask> Tasks { get; set; }
    }
}
