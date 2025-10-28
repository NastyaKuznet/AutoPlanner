using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Data.TaskData.Model.Answer
{
    public class GetByUserIdAnswerData
    {
        public ClassicAnswerStatus Status { get; }

        public List<TaskDatabase> Tasks { get; }

        public GetByUserIdAnswerData(
            ClassicAnswerStatus status, 
            List<TaskDatabase> tasks)
        {
            Status = status;
            Tasks = tasks;
        }
    }
}
