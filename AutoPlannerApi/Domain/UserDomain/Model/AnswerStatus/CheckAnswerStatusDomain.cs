using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Domain.UserDomain.Model.AnswerStatus
{
    public class CheckAnswerStatusDomain : ClassicAnswerStatus
    {
        public static readonly int UserExist = 2;
        public static readonly int UserNotExists = 3;
    }
}
