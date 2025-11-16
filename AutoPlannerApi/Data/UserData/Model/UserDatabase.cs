namespace AutoPlannerApi.Data.UserData.Model
{
    /// <summary>
    /// Класс пользователя для базы данных.
    /// </summary>
    public class UserDatabase
    {
        public int Id { get; }
        public string Nickname { get; }
        public string Password { get; }
        public long? TelegramChatId { get; }

        public UserDatabase(int id, string nickname, string password, long? telegramChatId = null)
        {
            Id = id;
            Nickname = nickname;
            Password = password;
            TelegramChatId = telegramChatId;
        }
    }
}
