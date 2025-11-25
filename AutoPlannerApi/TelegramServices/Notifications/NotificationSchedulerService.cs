using AutoPlannerApi.Data.NotificationData.Interface;
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
        private readonly ISentNotificationRepository _notificationRepository;

        public NotificationSchedulerService(
            ILogger<NotificationSchedulerService> logger,
            ITelegramLinkingService linkingService,
            ITelegramBotService botService,
            IUserDatabaseRepository userRepository,
            ISentNotificationRepository notificationRepository)
        {
            _logger = logger;
            _linkingService = linkingService;
            _botService = botService;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task ScheduleUserNotificationsAsync(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user?.TelegramChatId == null)
            {
                _logger.LogInformation("Пользователь {UserId} не имеет привязанного Telegram", userId);
                return;
            }

            var tasks = await _linkingService.GetUserTasksForNotification(userId);
            var chatId = user.TelegramChatId.Value;

            TimeZoneInfo ekbTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Yekaterinburg");

            _logger.LogInformation("Планирование уведомлений для пользователя {UserId}, задач: {TaskCount}", userId, tasks.Count);

            foreach (var task in tasks)
            {
                var alreadyExists = await _notificationRepository.ExistsAsync(userId, task.Id);
                if (alreadyExists)
                {
                    _logger.LogInformation("Пропускаем задачу '{TaskName}' - уведомление уже было", task.Name);
                    continue;
                }

                DateTime taskTimeUtc = TimeZoneInfo.ConvertTimeToUtc(
                    DateTime.SpecifyKind(task.StartDateTime, DateTimeKind.Unspecified),
                    ekbTimeZone);

                var notificationTime = taskTimeUtc.AddMinutes(-15);

                if (notificationTime > DateTime.UtcNow)
                {
                    BackgroundJob.Schedule(() =>
                        SendNotificationAsync(task, chatId), notificationTime);

                    await _notificationRepository.AddAsync(userId, task.Id);

                    _logger.LogInformation("Запланировано уведомление для задачи '{TaskName}' на {NotificationTime}",
                        task.Name, notificationTime);
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
                await ScheduleUserNotificationsAsync(user.Id);
            }
        }
    }
}
