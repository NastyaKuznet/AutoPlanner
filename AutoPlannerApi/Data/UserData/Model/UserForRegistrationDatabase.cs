namespace AutoPlannerApi.Data.UserData.Model
{
    public class UserForRegistrationDatabase
    {
        public string Nickname { get; }
        public string Password { get; }

        public UserForRegistrationDatabase(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }
    }
}
