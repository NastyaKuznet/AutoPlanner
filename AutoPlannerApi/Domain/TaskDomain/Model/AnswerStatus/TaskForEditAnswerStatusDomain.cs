using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Domain.TaskDomain.Model.AnswerStatus
{
    public class TaskForEditAnswerStatusDomain : ClassicAnswerStatus
    {
        public static readonly int TaskNotExist = 2;

        public static readonly int NotDeleteFromTimeTable = 3;
    }
}
