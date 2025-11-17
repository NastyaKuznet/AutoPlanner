using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Domain.UserDomain.Model.AnswerStatus
{
    public class AuthorizationAnswerStatusDomain : ClassicAnswerStatus
    {
        public readonly static int NicknameNotExist = 2;

        public readonly static int PasswordNotCorrect = 3;
    }
}
