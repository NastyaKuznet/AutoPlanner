namespace AutoPlannerApi.Data.NotificationData.Interface
{
    public interface ISentNotificationRepository
    {
        Task<bool> ExistsAsync(int userId, int taskId);
        Task<bool> AddAsync(int userId, int taskId);
        Task<bool> RemoveAsync(int userId, int taskId);
        Task<bool> RemoveByUserAsync(int userId);
        Task<bool> RemoveByTaskAsync(int taskId);
    }
}
