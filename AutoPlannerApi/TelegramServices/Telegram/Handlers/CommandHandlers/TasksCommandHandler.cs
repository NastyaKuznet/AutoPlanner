using AutoPlannerApi.Data.UserData.Interface;
using AutoPlannerApi.Domain.TimeTableDomain.Model;
using AutoPlannerApi.Domain.UserDomain.Interface;

namespace AutoPlannerApi.TelegramServices.Telegram.Handlers.CommandHandlers
{
    public class TasksCommandHandler
    {
        private readonly IUserDatabaseRepository _userRepository;
        private readonly ITelegramLinkingService _linkingService;
        private readonly ITelegramBotService _botService;
        private readonly ILogger<TasksCommandHandler> _logger;

        public TasksCommandHandler(
            IUserDatabaseRepository userRepository,
            ITelegramLinkingService linkingService,
            ITelegramBotService botService,
            ILogger<TasksCommandHandler> logger)
        {
            _userRepository = userRepository;
            _linkingService = linkingService;
            _botService = botService;
            _logger = logger;
        }

        public async Task HandleAsync(long chatId)
        {
            _logger.LogInformation("Обработка команды /tasks от {ChatId}", chatId);

            try
            {
                var user = await _userRepository.GetUserByTelegramChatId(chatId);
                if (user == null)
                {
                    await SendNotLinkedMessageAsync(chatId);
                    return;
                }

                var tasks = await _linkingService.GetUserTasksForNotification(user.Id);

                if (!tasks.Any())
                {
                    await SendNoTasksMessageAsync(chatId);
                    return;
                }

                await SendTasksListAsync(chatId, tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении задач для chatId {ChatId}", chatId);
                await SendErrorMessageAsync(chatId);
            }
        }

        private async Task SendNotLinkedMessageAsync(long chatId)
        {
            await _botService.SendMessageAsync(chatId,
                "Аккаунт не привязан\n\n" +
                "Для просмотра задач необходимо привязать ваш аккаунт.\n\n" +
                "Как привязать:\n" +
                "1. Откройте веб-приложение AutoPlanner\n" +
                "2. Получите код привязки\n" +
                "3. Используйте команду: `/start YOUR_CODE`\n\n" +
                "После привязки вы сможете просматривать свои задачи здесь.");
        }

        private async Task SendNoTasksMessageAsync(long chatId)
        {
            await _botService.SendMessageAsync(chatId,
                "Нет активных задач\n\n" +
                "У вас нет активных задач на ближайшее время.\n\n" +
                "Что можно сделать:\n" +
                "• Добавить новые задачи в веб-приложении\n" +
                "• Проверить выполненные задачи\n" +
                "• Настроить расписание");
        }

        private async Task SendTasksListAsync(long chatId, List<TimeTableItemDomain> tasks)
        {
            var message = "Ваши активные задачи:\n\n";

            var todayTasks = tasks.Where(t => t.StartDateTime.Date == DateTime.Today)
                                .OrderBy(t => t.StartDateTime)
                                .ToList();

            var futureTasks = tasks.Where(t => t.StartDateTime.Date > DateTime.Today)
                                 .OrderBy(t => t.StartDateTime)
                                 .ToList();

            if (todayTasks.Any())
            {
                message += "Сегодня:\n";
                foreach (var task in todayTasks)
                {
                    message += FormatTask(task) + "\n";
                }
                message += "\n";
            }

            if (futureTasks.Any())
            {
                message += "Будущие задачи:\n";
                foreach (var task in futureTasks.Take(10))
                {
                    message += FormatTask(task) + "\n";
                }

                if (futureTasks.Count > 10)
                {
                    message += $"\n... и еще {futureTasks.Count - 10} задач\n";
                }
            }

            message += $"\nВсего задач: {tasks.Count}";

            await _botService.SendMessageAsync(chatId, message);
        }

        private string FormatTask(TimeTableItemDomain task)
        {
            TimeZoneInfo ekbTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Yekaterinburg");

            DateTime taskTimeUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(task.StartDateTime, DateTimeKind.Unspecified),
                ekbTimeZone);
            var timeLeft = taskTimeUtc - DateTime.UtcNow;

            return $"{task.Name}\n" +
                   $"{task.StartDateTime:dd.MM.yyyy HH:mm} " +
                   $"(через {FormatTimeSpan(timeLeft)})\n" +
                   $"Приоритет: {task.Priority}/10\n";
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays}д {timeSpan.Hours}ч";
            if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours}ч {timeSpan.Minutes}м";
            return $"{timeSpan.Minutes}м";
        }

        private async Task SendErrorMessageAsync(long chatId)
        {
            await _botService.SendMessageAsync(chatId,
                "Не удалось загрузить задачи\n\n" +
                "Пожалуйста, попробуйте позже или обратитесь в поддержку.");
        }
    }
}
