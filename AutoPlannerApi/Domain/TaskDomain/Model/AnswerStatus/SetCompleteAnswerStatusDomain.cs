using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Domain.TaskDomain.Model.AnswerStatus
{
    public class SetCompleteAnswerStatusDomain : ClassicAnswerStatus
    {
        public static readonly int TaskNotExist = 2;
        public static readonly int TimeTableNotExist = 3;
    }
}
