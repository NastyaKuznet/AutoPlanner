using AutoPlannerApi.Domain.TaskDomain.Model.AnswerStatus;

namespace AutoPlannerApi.Domain.TaskDomain.Model.Answer
{
    public class GetByUserIdAnswerDomain
    {
        public GetByUserIdAnswerStatusDomain Status { get; }

        public List<TaskGetDomain> Tasks { get; }

        public GetByUserIdAnswerDomain(
            GetByUserIdAnswerStatusDomain status,  
            List<TaskGetDomain> tasks)
        {
            Status = status;
            Tasks = tasks;
        }
    }
}
