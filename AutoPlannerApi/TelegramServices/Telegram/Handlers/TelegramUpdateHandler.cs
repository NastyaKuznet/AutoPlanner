using AutoPlannerApi.Domain.UserDomain.Interface;
using AutoPlannerApi.TelegramServices.Telegram.Handlers.CommandHandlers;
using Telegram.Bot.Types;

namespace AutoPlannerApi.TelegramServices.Telegram.Handlers
{
    public class TelegramUpdateHandler
    {
        private readonly ILogger<TelegramUpdateHandler> _logger;
        private readonly ITelegramLinkingService _linkingService;
        private readonly StartCommandHandler _startHandler;
        private readonly TasksCommandHandler _tasksHandler;
        private readonly HelpCommandHandler _helpHandler;

        public TelegramUpdateHandler(
            ILogger<TelegramUpdateHandler> logger,
            ITelegramLinkingService linkingService,
            StartCommandHandler startHandler,
            TasksCommandHandler tasksHandler,
            HelpCommandHandler helpHandler)
        {
            _logger = logger;
            _linkingService = linkingService;
            _startHandler = startHandler;
            _tasksHandler = tasksHandler;
            _helpHandler = helpHandler;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            if (update.Message is not { } message)
                return;

            var chatId = message.Chat.Id;
            var messageText = message.Text ?? string.Empty;

            _logger.LogInformation("Обработка сообщения от {ChatId}: {Text}", chatId, messageText);

            try
            {
                if (messageText.StartsWith("/start"))
                {
                    await _startHandler.HandleAsync(chatId, messageText);
                }
                else if (messageText.StartsWith("/tasks"))
                {
                    await _tasksHandler.HandleAsync(chatId);
                }
                else if (messageText.StartsWith("/help"))
                {
                    await _helpHandler.HandleAsync(chatId);
                }
                else
                {
                    await _startHandler.SendUnknownCommandAsync(chatId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки сообщения от {ChatId}", chatId);
                await _startHandler.SendErrorMessageAsync(chatId);
            }
        }
    }
}
