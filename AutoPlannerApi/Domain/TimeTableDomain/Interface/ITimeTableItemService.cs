using AutoPlannerApi.Domain.TimeTableDomain.Model;
using AutoPlannerApi.Domain.TimeTableDomain.Model.Answer;
using AutoPlannerApi.Domain.TimeTableDomain.Model.Answer.AnswerStatus;

namespace AutoPlannerApi.Domain.TimeTableDomain.Interface
{
    public interface ITimeTableItemService
    {
        public Task<(GetTTByUserIdAnswerStatusDomain, List<TimeTableItemDomain>, List<PlanningTaskDomain>)> Get(int userId);
        
        public Task<RecreateTimeTableAnswerDomain> Recreate(int userId, DateTime startTimeTable, DateTime endDateTime);

        public Task<DeleteItemFormTimeTableAnswerStatusDomain> Delete(int taskId);
    }
}
