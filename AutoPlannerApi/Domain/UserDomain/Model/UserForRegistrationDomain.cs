namespace AutoPlannerApi.Domain.UserDomain.Model
{
    public class UserForRegistrationDomain
    {
        public string Nickname { get; set; }
        public string Password { get; set; }

        public UserForRegistrationDomain(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }
    }
}
