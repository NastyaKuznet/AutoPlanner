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

        public UserDatabase(int id, string nickname, string password)
        {
            Id = id;
            Nickname = nickname;
            Password = password;
        }
    }
}
