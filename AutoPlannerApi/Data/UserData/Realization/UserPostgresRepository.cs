using AutoPlannerApi.Data.UserData.Interface;
using AutoPlannerApi.Data.UserData.Model;
using AutoPlannerApi.Data.UserData.Model.AnswerStatus;

namespace AutoPlannerApi.Data.UserData.Realization
{
    public class UserPostgresRepository : IUserDatabaseRepository
    {
        public Task<CheckAnswerStatusDatabase> Check(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<UserDatabase>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public Task Registrate(UserForRegistrationDatabase userForRegistration)
        {
            throw new NotImplementedException();
        }
    }
}
