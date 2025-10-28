using AutoPlannerApi.Domain.UserDomain.Model;
using AutoPlannerApi.Domain.UserDomain.Model.AnswerStatus;

namespace AutoPlannerApi.Domain.UserDomain.Interface
{
    public interface IUserService
    {
        public Task<RegistrationAnswerStatusDomain> Registrate(UserForRegistrationDomain userForRegistration);

        public Task<CheckAnswerStatusDomain> Check(int userId);
    }
}
