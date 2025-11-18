using AutoPlannerApi.Data.UserData.Interface;
using AutoPlannerApi.Domain.UserDomain.Interface;
using AutoPlannerApi.TelegramServices.Notifications;

namespace AutoPlannerApi.TelegramServices.Telegram.Handlers.CommandHandlers
{
    public class StartCommandHandler
    {
        private readonly ITelegramLinkingService _linkingService;
        private readonly ITelegramBotService _botService;
        private readonly INotificationSchedulerService _notificationService;
        private readonly IUserDatabaseRepository _userRepository;
        private readonly ILogger<StartCommandHandler> _logger;

        public StartCommandHandler(
            ITelegramLinkingService linkingService,
            ITelegramBotService botService,
            INotificationSchedulerService notificationService,
            IUserDatabaseRepository userRepository,
            ILogger<StartCommandHandler> logger)
        {
            _linkingService = linkingService;
            _botService = botService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task HandleAsync(long chatId, string messageText)
        {
            _logger.LogInformation("Обработка команды /start от {ChatId}", chatId);

            var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                await SendWelcomeMessageAsync(chatId);
                return;
            }

            var code = parts[1];
            await HandleLinkingAsync(chatId, code);
        }

        private async Task SendWelcomeMessageAsync(long chatId)
        {
            var message = "Добро пожаловать в AutoPlanner Notifier Bot!\n\n" +
                         "Этот бот будет отправлять вам уведомления о ваших задачах.\n\n" +
                         "Для привязки аккаунта:\n" +
                         "1. Откройте веб-приложение AutoPlanner\n" +
                         "3. Нажмите \"Привязать Telegram\"\n";

            await _botService.SendMessageAsync(chatId, message);
        }

        private async Task HandleLinkingAsync(long chatId, string code)
        {
            try
            {
                _logger.LogInformation("Попытка привязки по коду {Code} для chatId {ChatId}", code, chatId);

                var existingUser = await _userRepository.GetUserByTelegramChatId(chatId);
                if (existingUser != null)
                {
                    await _botService.SendMessageAsync(chatId,
                        "Этот Telegram аккаунт уже привязан к другому пользователю.\n" +
                        "Если это ошибка, обратитесь в поддержку: @slapa7, @valecttgxyj, @Lu_i_z_a.");
                    return;
                }

                var result = await _linkingService.LinkUserToTelegram(code, chatId);

                if (result.Success)
                {
                    _logger.LogInformation("Успешная привязка пользователя {UserId} к chatId {ChatId}",
                        result.UserId, chatId);

                    await _notificationService.ScheduleUserNotificationsAsync(result.UserId, chatId);

                    await _botService.SendMessageAsync(chatId,
                        $"Привет, {result.UserName}!\n\n" +
                        "Ваш аккаунт успешно привязан!\n\n" +
                        "Теперь вы будете получать:\n" +
                        "• Уведомления за 15 минут до начала задач\n" +
                        "Используйте `/tasks` для просмотра ваших задач\n" +
                        "Используйте `/help` для справки по командам");

                    _logger.LogInformation("Уведомления запланированы для пользователя {UserId}", result.UserId);
                }
                else
                {
                    _logger.LogWarning("Ошибка привязки по коду {Code}: {Error}", code, result.ErrorMessage);

                    await _botService.SendMessageAsync(chatId,
                        $"Ошибка привязки\n\n" +
                        $"Причина: {result.ErrorMessage}\n\n" +
                        "Попробуйте следующее:\n" +
                        "1. Проверьте правильность кода\n" +
                        "2. Убедитесь, что код не просрочен (действует 10 минут)\n" +
                        "3. Получите новый код в веб-приложении\n" +
                        "4. Попробуйте снова: `/start NEW_CODE`");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке привязки для chatId {ChatId}", chatId);

                await _botService.SendMessageAsync(chatId,
                    "Произошла непредвиденная ошибка\n\n" +
                    "Пожалуйста, попробуйте позже или обратитесь в поддержку: @slapa7, @valecttgxyj, @Lu_i_z_a.");
            }
        }

        public async Task SendUnknownCommandAsync(long chatId)
        {
            await _botService.SendMessageAsync(chatId,
                "Неизвестная команда\n\n" +
                "Используйте /help для просмотра доступных команд.");
        }

        public async Task SendErrorMessageAsync(long chatId)
        {
            await _botService.SendMessageAsync(chatId,
                "Произошла ошибка\n\n" +
                "Пожалуйста, попробуйте еще раз или обратитесь в поддержку.");
        }
    }
}
