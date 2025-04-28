using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VolunteerManagment.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("CreatedBy")]
        public int CreatedBy { get; set; }
        public User Organizer { get; set; }
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<VolunteerEvent> Volunteers { get; set; } = new List<VolunteerEvent>();
        public ICollection<EventTask> Tasks { get; set; } = new List<EventTask>();

    }
}
