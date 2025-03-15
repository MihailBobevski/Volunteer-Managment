using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VolunteerManagment.Models
{
    public class EventTask
    {
        [Key]
        public int TaskId { get; set; }

        [ForeignKey("EventId")]
        public int EventId { get; set; }
        public Event Event { get; set; }

        [ForeignKey("AssignedTo")]
        public int? AssignedTo { get; set; }
        public User User { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        public DateTime Deadline { get; set; }
    }
}
