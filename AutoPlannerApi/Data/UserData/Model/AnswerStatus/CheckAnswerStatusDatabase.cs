using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Data.UserData.Model.AnswerStatus
{
    public class CheckAnswerStatusDatabase : ClassicAnswerStatus
    {
        public static readonly int UserExist = 2;

        public static readonly int UserNotExist = 3;
    }
}
