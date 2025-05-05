using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace VolunteerManagment.Models
{
    public class EventTask
    {
        [Key]
        public int TaskId { get; set; }

        [ForeignKey("EventId")]
        public int EventId { get; set; }
        
        [ValidateNever]
        public Event Event { get; set; }

        [ForeignKey("AssignedTo")]
        public int? AssignedTo { get; set; }
        [ValidateNever]
        public User User { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime Deadline { get; set; }
    }
}
