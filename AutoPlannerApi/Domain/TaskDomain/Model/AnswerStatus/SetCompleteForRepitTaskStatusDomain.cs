using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Domain.TaskDomain.Model.AnswerStatus
{
    public class SetCompleteForRepitTaskStatusDomain : ClassicAnswerStatus
    {
        public static readonly int TaskNotExist = 2;
    }
}
