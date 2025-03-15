using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace VolunteerManagment.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }

        [ForeignKey("EventId")]
        public int EventId { get; set; }
        public Event Event { get; set; }

        [ForeignKey("GeneratedBy")]
        public int GeneratedBy { get; set; }
        public User User { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}


