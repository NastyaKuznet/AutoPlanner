using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoPlannerApi.TelegramServices.Telegram
{
    public interface ITelegramBotService
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task SendMessageAsync(long chatId, string message);
        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken);
    }
}
