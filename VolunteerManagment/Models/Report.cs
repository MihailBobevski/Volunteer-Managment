using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace VolunteerManagment.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }

        [Required(ErrorMessage = "Report content is required.")]
        public string Content { get; set; }

        public int EventId { get; set; }

        [ValidateNever]
        public Event Event { get; set; }

        public int UserId { get; set; }

        [ValidateNever]
        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}