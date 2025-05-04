namespace VolunteerManagment.Services
{
    public interface ITaskService
    {
        bool AssignTaskToUser(int taskId, int eventId, string username);
    }
}