using AutoPlannerApi.Domain.TimeTableDomain.Model.Answer.AnswerStatus;

namespace AutoPlannerApi.Domain.TimeTableDomain.Model.Answer
{
    public class RecreateTimeTableAnswerDomain
    {
        public RecreateTimeTableAnswerStatusDomain Status { get; }

        public List<TimeTableItemDomain> TimeTableItems { get; }

        public List<PlanningTaskDomain> PenaltyTasks { get; }

        public RecreateTimeTableAnswerDomain(
            RecreateTimeTableAnswerStatusDomain status, 
            List<TimeTableItemDomain> timeTableItems, 
            List<PlanningTaskDomain> penaltyTasks)
        {
            Status = status;
            TimeTableItems = timeTableItems;
            PenaltyTasks = penaltyTasks;
        }
    }
}
