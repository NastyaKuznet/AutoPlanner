using AutoPlannerApi.Data.UserData.Interface;
using AutoPlannerApi.Domain.TimeTableDomain.Model;
using AutoPlannerApi.Domain.UserDomain.Interface;
using AutoPlannerApi.TelegramServices.Telegram;
using Hangfire;

namespace AutoPlannerApi.TelegramServices.Notifications
{
    public class NotificationSchedulerService : INotificationSchedulerService
    {
        private readonly ILogger<NotificationSchedulerService> _logger;
        private readonly ITelegramLinkingService _linkingService;
        private readonly ITelegramBotService _botService;
        private readonly IUserDatabaseRepository _userRepository;

        public NotificationSchedulerService(
            ILogger<NotificationSchedulerService> logger,
            ITelegramLinkingService linkingService,
            ITelegramBotService botService,
            IUserDatabaseRepository userRepository)
        {
            _logger = logger;
            _linkingService = linkingService;
            _botService = botService;
            _userRepository = userRepository;
        }

        public async Task ScheduleUserNotificationsAsync(int userId, long chatId)
        {
            var tasks = await _linkingService.GetUserTasksForNotification(userId);

            TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Yekaterinburg");

            foreach (var task in tasks)
            {
                DateTime taskTimeUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(task.StartDateTime, DateTimeKind.Unspecified), moscowTimeZone);

                var notificationTime = taskTimeUtc.AddMinutes(-15);
                if (notificationTime > DateTime.UtcNow)
                {
                    BackgroundJob.Schedule(() =>
                        SendNotificationAsync(task, chatId), notificationTime);
                }
            }
        }

        public async Task SendNotificationAsync(TimeTableItemDomain task, long chatId)
        {
            var message = $"Напоминание: {task.Name}\n" +
                         $"Начало: {task.StartDateTime:HH:mm}\n" +
                         $"Приоритет: {task.Priority}";

            await _botService.SendMessageAsync(chatId, message);
        }

        public async Task RescheduleAllNotificationsAsync()
        {
            var users = await _userRepository.GetUsers();
            var usersWithTelegram = users.Where(u => u.TelegramChatId.HasValue);

            foreach (var user in usersWithTelegram)
            {
                await ScheduleUserNotificationsAsync(user.Id, user.TelegramChatId.Value);
            }
        }
    }
}
