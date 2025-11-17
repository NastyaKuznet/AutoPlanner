using AutoPlannerApi.Data.UserData.Interface;
using AutoPlannerApi.Data.UserData.Model;
using AutoPlannerApi.Data.UserData.Model.Answer;
using AutoPlannerApi.Data.UserData.Model.AnswerStatus;

namespace AutoPlannerApi.Data.UserData.Realization
{
    public class UserRamRepository : IUserDatabaseRepository
    {
        private List<UserDatabase> _users = new List<UserDatabase>();
        private int _userId = 1;
        public Task<int> Registrate(UserForRegistrationAndAuthorizationDatabase userForRegistration)
        {
            var id = _userId++;
            _users.Add(new UserDatabase(
                id, 
                userForRegistration.Nickname, 
                userForRegistration.Password));
            return Task.FromResult(id);
        }

        public Task<IReadOnlyCollection<UserDatabase>> GetUsers()
        {
            return Task.FromResult((IReadOnlyCollection<UserDatabase>)_users.AsReadOnly());
        }

        public Task<CheckAnswerStatusDatabase> Check(int userId)
        {
            foreach (var user in _users)
            {
                if (user.Id == userId)
                {
                    return Task.FromResult(new CheckAnswerStatusDatabase() { Status = CheckAnswerStatusDatabase.UserExist });
                }
            }
            return Task.FromResult(new CheckAnswerStatusDatabase() { Status = CheckAnswerStatusDatabase.UserNotExist });
        }
    }
}
