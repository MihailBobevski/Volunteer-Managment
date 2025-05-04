using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteerManagment.Models
{
    public class OrganizerRequest
    {
        [Key]
        public int RequestId { get; set; }
        
        
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        
        [Required]
        public DateTime DateRequested { get; set; } = DateTime.Now;
        
        [Required]
        [MaxLength(1000)]
        public string MotivationalLetter { get; set; }

        [Required] 
        [MaxLength(20)] 
        public string Status { get; set; } = "Pending";
    }
}