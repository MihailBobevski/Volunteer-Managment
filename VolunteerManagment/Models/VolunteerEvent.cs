using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VolunteerManagment.Models
{
    public class VolunteerEvent
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("EventId")]
        public int EventId { get; set; }
        public Event Event { get; set; }

        [Required]
        public string Status { get; set; }

        public decimal HoursContributed { get; set; }
    }
}

