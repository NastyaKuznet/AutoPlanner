using AutoPlannerApi.Domain.UserDomain.Model;
using AutoPlannerApi.Domain.UserDomain.Model.Answer;
using AutoPlannerApi.Domain.UserDomain.Model.AnswerStatus;

namespace AutoPlannerApi.Domain.UserDomain.Interface
{
    public interface IUserService
    {
        public Task<(RegistrationAnswerStatusDomain, int)> Registrate(UserForRegistrationAndAuthorizationDomain userForRegistration);

        public Task<CheckAnswerStatusDomain> Check(int userId);

        public Task<AuthorizationAnswerDomain> Authorization(UserForRegistrationAndAuthorizationDomain userForAuthorization);
    }
}
