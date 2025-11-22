namespace AutoPlannerApi.Domain.UserDomain.Model
{
    public class UserForRegistrationAndAuthorizationDomain
    {
        public string Nickname { get; set; }
        public string Password { get; set; }

        public UserForRegistrationAndAuthorizationDomain(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }
    }
}
