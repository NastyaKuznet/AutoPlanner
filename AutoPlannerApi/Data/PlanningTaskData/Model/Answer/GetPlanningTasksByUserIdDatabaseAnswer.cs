using AutoPlannerApi.Data.PlanningTaskData.Model.Answer.AnswerStatus;

namespace AutoPlannerApi.Data.PlanningTaskData.Model.Answer
{
    public class GetPlanningTasksByUserIdDatabaseAnswer
    {
        public GetPlanningTasksByUserIdDatabaseAnswerStatus Status { get; set; }

        public List<PlanningTaskDatabase> PlanningTasks { get; set; }
    }
}
