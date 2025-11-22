namespace AutoPlannerApi.TelegramServices.Telegram.Handlers.CommandHandlers
{
    public class HelpCommandHandler
    {
        private readonly ITelegramBotService _botService;

        public HelpCommandHandler(ITelegramBotService botService)
        {
            _botService = botService;
        }

        public async Task HandleAsync(long chatId)
        {
            var helpMessage = "Доступные команды:\n" +
                             "/start - Приветствие и инструкция по привязке\n" +
                             "/start CODE - Привязать аккаунт с кодом\n" +
                             "/tasks - Показать ваши активные задачи\n" +
                             "/help - Показать эту справку\n\n" +
                             "Уведомления:\n" +
                             "• За 15 минут до начала каждой задачи\n\n" +
                             "Если что-то не работает:\n" +
                             "1. Проверьте привязку аккаунта\n" +
                             "2. Убедитесь, что у вас есть активные задачи\n" +
                             "3. Обратитесь в поддержку";

            await _botService.SendMessageAsync(chatId, helpMessage);
        }
    }
}
