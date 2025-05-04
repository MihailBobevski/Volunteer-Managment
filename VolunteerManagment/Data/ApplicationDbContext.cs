using Microsoft.EntityFrameworkCore;
using VolunteerManagment.Models;

namespace VolunteerManagment2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<VolunteerEvent> VolunteersEvents { get; set; }
        public DbSet<EventTask> Tasks { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<OrganizerRequest> OrganizerRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VolunteerEvent>()
                .HasOne(ve => ve.User)
                .WithMany(u => u.VolunteerEvents)
                .HasForeignKey(ve => ve.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VolunteerEvent>()
                .HasOne(ve => ve.Event)
                .WithMany(e => e.Volunteers)
                .HasForeignKey(ve => ve.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventTask>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tasks)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventTask>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Reports)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
