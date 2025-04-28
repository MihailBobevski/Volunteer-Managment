using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace VolunteerManagment.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

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
        [MinLength(8)]
        public string Password { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; } = 1;
        [ValidateNever]
        public Roles Role { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<VolunteerEvent> VolunteerEvents { get; set; } = new List<VolunteerEvent>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<EventTask> Tasks { get; set;} = new List<EventTask>();
    }
}
