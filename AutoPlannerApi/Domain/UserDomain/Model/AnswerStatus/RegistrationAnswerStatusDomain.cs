using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Domain.UserDomain.Model.AnswerStatus
{
    public class RegistrationAnswerStatusDomain : ClassicAnswerStatus
    {
        public static readonly int NicknameAlreadyExist = 2;
        public static readonly int PasswordValidation = 3;

    }
}
