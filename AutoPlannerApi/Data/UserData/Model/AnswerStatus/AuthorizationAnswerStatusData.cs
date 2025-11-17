using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Data.UserData.Model.AnswerStatus
{
    public class AuthorizationAnswerStatusData : ClassicAnswerStatus
    {
        public readonly static int NicknameNotExist = 2;
        public readonly static int PasswordNotCorrect = 3;
    }
}
