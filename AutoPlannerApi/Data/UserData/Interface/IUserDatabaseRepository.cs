using AutoPlannerApi.Data.UserData.Model;
using AutoPlannerApi.Data.UserData.Model.AnswerStatus;

namespace AutoPlannerApi.Data.UserData.Interface
{
    public interface IUserDatabaseRepository
    {
        public Task Registrate(UserForRegistrationDatabase userForRegistration);
        public Task<IReadOnlyCollection<UserDatabase>> GetUsers();

        public Task<CheckAnswerStatusDatabase> Check(int userId);
        Task<UserDatabase> GetUserByTelegramChatId(long chatId);
        Task<bool> UpdateUserTelegramChatId(int userId, long chatId);
        Task<UserDatabase> GetUserById(int userId);
    }
}
