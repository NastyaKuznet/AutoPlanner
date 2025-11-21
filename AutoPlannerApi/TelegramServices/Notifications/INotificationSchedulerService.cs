namespace AutoPlannerApi.TelegramServices.Notifications
{
    public interface INotificationSchedulerService
    {
        Task ScheduleUserNotificationsAsync(int userId);
        Task RescheduleAllNotificationsAsync();
    }
}
