using AutoPlannerApi.Domain.TimeTableDomain.Model;

namespace AutoPlannerApi.Controllers.Model
{
    public class RecreateTimeTableAnswer
    {
        public List<TimeTableItemDomain> TimeTableItems { get; }

        public List<PlanningTaskDomain> PenaltyTasks { get; }

        public RecreateTimeTableAnswer(
            List<TimeTableItemDomain> timeTableItems,
            List<PlanningTaskDomain> penaltyTasks) 
        {
            TimeTableItems = timeTableItems;
            PenaltyTasks = penaltyTasks;
        }
    }
}
