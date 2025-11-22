using AutoPlannerApi.Data.UserData.Interface;
using AutoPlannerApi.Data.UserData.Model;
using AutoPlannerApi.Data.UserData.Model.AnswerStatus;
using AutoPlannerApi.Domain.UserDomain.Interface;
using AutoPlannerApi.Domain.UserDomain.Model;
using AutoPlannerApi.Domain.UserDomain.Model.Answer;
using AutoPlannerApi.Domain.UserDomain.Model.AnswerStatus;

namespace AutoPlannerApi.Domain.UserDomain.Realization
{
    public class UserClassicService : IUserService
    {
        private IUserDatabaseRepository _userDatabaseRepository;
        public UserClassicService(IUserDatabaseRepository userDatabaseRepository) 
        {
            _userDatabaseRepository = userDatabaseRepository;
        }

        public async Task<AuthorizationAnswerDomain> Authorization(UserForRegistrationAndAuthorizationDomain userForAuthorization)
        {
            var users = await _userDatabaseRepository.GetUsers();
            foreach (var user in users)
            {
                if (user.Nickname == userForAuthorization.Nickname)
                {
                    if (user.Password == userForAuthorization.Password)
                    {
                        return new AuthorizationAnswerDomain(new AuthorizationAnswerStatusDomain() { Status = AuthorizationAnswerStatusDomain.Good }, user.Id);
                    }
                    return new AuthorizationAnswerDomain(new AuthorizationAnswerStatusDomain() { Status = AuthorizationAnswerStatusDomain.PasswordNotCorrect }, -1);
                }
            }
            return new AuthorizationAnswerDomain(new AuthorizationAnswerStatusDomain() { Status = AuthorizationAnswerStatusDomain.NicknameNotExist }, -1);
        }

        public async Task<CheckAnswerStatusDomain> Check(int userId)
        {
            var checkUserToRepository = await _userDatabaseRepository.Check(userId);
            var mapDataStatusToDomainStatus = new Dictionary<int, int>() 
            {
                { CheckAnswerStatusDatabase.Good, CheckAnswerStatusDomain.Good },
                { CheckAnswerStatusDatabase.Bad, CheckAnswerStatusDomain.Bad },
                { CheckAnswerStatusDatabase.UserNotExist, CheckAnswerStatusDomain.UserNotExists },
                { CheckAnswerStatusDomain.UserExist, CheckAnswerStatusDomain.UserExist },
            };

            return new CheckAnswerStatusDomain()
            {
                Status = mapDataStatusToDomainStatus[checkUserToRepository.Status],
            };
        }

        public async Task<(RegistrationAnswerStatusDomain, int)> Registrate(UserForRegistrationAndAuthorizationDomain userForRegistration)
        {
            var users = await _userDatabaseRepository.GetUsers();
            foreach (var user in users)
            {
                if (user.Nickname.Equals(userForRegistration.Nickname))
                {
                    return (new RegistrationAnswerStatusDomain { Status = RegistrationAnswerStatusDomain.NicknameAlreadyExist }, -1);
                }
            }
            // TODO validation for example
            var id = await _userDatabaseRepository.Registrate(
                new UserForRegistrationAndAuthorizationDatabase(
                    userForRegistration.Nickname,
                    userForRegistration.Password));
            return (new RegistrationAnswerStatusDomain { Status = Data.Common.Model.ClassicAnswerStatus.Good }, id);
        }
    }
}
