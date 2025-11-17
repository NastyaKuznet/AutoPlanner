namespace AutoPlannerApi.Data.UserData.Model
{
    public class UserForRegistrationAndAuthorizationDatabase
    {
        public string Nickname { get; }
        public string Password { get; }

        public UserForRegistrationAndAuthorizationDatabase(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }
    }
}
