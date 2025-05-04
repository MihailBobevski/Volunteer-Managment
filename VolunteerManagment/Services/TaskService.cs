using VolunteerManagment.Models;
using VolunteerManagment2.Data;

namespace VolunteerManagment.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AssignTaskToUser(int taskId, int eventId, string username)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);
            if (task == null || task.AssignedTo != null)
                return false;

            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
                return false;

            task.AssignedTo = user.UserId;
            task.Status = "Assigned";

            _context.SaveChanges();
            return true;
        }
    }
}