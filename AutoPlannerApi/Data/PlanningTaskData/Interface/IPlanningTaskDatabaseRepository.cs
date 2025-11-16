using AutoPlannerApi.Data.PlanningTaskData.Model;
using AutoPlannerApi.Data.PlanningTaskData.Model.Answer;

namespace AutoPlannerApi.Data.PlanningTaskData.Interface
{
    public interface IPlanningTaskDatabaseRepository
    {
        public Task<GetPlanningTasksByUserIdDatabaseAnswer> Get(int userId);

        public Task<AddPlanningTaskDatabaseAnswer> Add(PlanningTaskDatabase planningTask);

        public Task<DeletePlanningTaskDatabaseAnswer> DeleteByUserId(int userId);
    }
}
