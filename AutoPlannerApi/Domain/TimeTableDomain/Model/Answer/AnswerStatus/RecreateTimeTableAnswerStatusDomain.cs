using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Domain.TimeTableDomain.Model.Answer.AnswerStatus
{
    public class RecreateTimeTableAnswerStatusDomain : ClassicAnswerStatus
    {
        public static readonly int UserNotExist = 2;

        public static readonly int BadFoundUser = 3;

        public static readonly int NotFullLoadTask = 4;
    }
}
