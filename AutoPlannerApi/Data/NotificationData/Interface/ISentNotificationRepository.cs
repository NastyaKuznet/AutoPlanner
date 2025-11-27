namespace AutoPlannerApi.Data.NotificationData.Interface
{
    public interface ISentNotificationRepository
    {
        Task<bool> ExistsAsync(int userId, int taskId);
        Task<bool> AddAsync(int userId, int taskId, string jobId);
        Task<List<string>> RemoveByUserAsync(int userId);
    }
}
