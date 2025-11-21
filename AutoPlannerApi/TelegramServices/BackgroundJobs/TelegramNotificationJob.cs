using AutoPlannerApi.TelegramServices.Notifications;
using Hangfire;

namespace AutoPlannerApi.TelegramServices.BackgroundJobs
{
    public class TelegramNotificationJob
    {
        private readonly INotificationSchedulerService _notificationService;
        private readonly ILogger<TelegramNotificationJob> _logger;

        public TelegramNotificationJob(
            INotificationSchedulerService notificationService,
            ILogger<TelegramNotificationJob> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task RescheduleAllNotifications()
        {
            _logger.LogInformation("Запуск перепланирования всех уведомлений");
            await _notificationService.RescheduleAllNotificationsAsync();
            _logger.LogInformation("Перепланирование уведомлений завершено");
        }
    }
}
