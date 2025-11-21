using AutoPlannerApi.TelegramServices.Telegram.Handlers;
using AutoPlannerApi.TelegramServices.Telegram.Models;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions;

namespace AutoPlannerApi.TelegramServices.Telegram
{
    public class TelegramBotService : ITelegramBotService, IHostedService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<TelegramBotService> _logger;
        private readonly TelegramBotConfig _config;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _cts;

        public TelegramBotService(
            IOptions<TelegramBotConfig> config,
            ILogger<TelegramBotService> logger,
            IServiceProvider serviceProvider)
        {
            _config = config.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _botClient = new TelegramBotClient(_config.BotToken);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Запуск Telegram бота...");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: _cts.Token
            );

            _logger.LogInformation("Telegram бот запущен");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Остановка Telegram бота...");
            _cts?.Cancel();
            _logger.LogInformation("Telegram бот остановлен");
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var telegramUpdateHandler = scope.ServiceProvider.GetRequiredService<TelegramUpdateHandler>();
            await telegramUpdateHandler.HandleUpdateAsync(update);
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Ошибка в Telegram боте");
        }

        public async Task SendMessageAsync(long chatId, string message)
        {
            try
            {
                await _botClient.SendMessage(
                    chatId: chatId,
                    text: message,
                    cancellationToken: CancellationToken.None);

                _logger.LogDebug("Сообщение отправлено в чат {ChatId}", chatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка отправки сообщения в чат {ChatId}", chatId);
            }
        }
    }
}
