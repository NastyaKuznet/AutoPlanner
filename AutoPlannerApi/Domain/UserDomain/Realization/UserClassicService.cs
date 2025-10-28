using AutoPlannerApi.Data.UserData.Interface;
using AutoPlannerApi.Data.UserData.Model;
using AutoPlannerApi.Data.UserData.Model.AnswerStatus;
using AutoPlannerApi.Domain.UserDomain.Interface;
using AutoPlannerApi.Domain.UserDomain.Model;
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

        public async Task<RegistrationAnswerStatusDomain> Registrate(UserForRegistrationDomain userForRegistration)
        {
            var users = await _userDatabaseRepository.GetUsers();
            foreach (var user in users)
            {
                if (user.Nickname.Equals(userForRegistration.Nickname))
                {
                    return new RegistrationAnswerStatusDomain { Status = RegistrationAnswerStatusDomain.NicknameAlreadyExist };
                }
            }
            // TODO validation for example
            await _userDatabaseRepository.Registrate(
                new UserForRegistrationDatabase(
                    userForRegistration.Nickname,
                    userForRegistration.Password));
            return new RegistrationAnswerStatusDomain { Status = Data.Common.Model.ClassicAnswerStatus.Good };
        }
    }
}
