using AutoPlannerApi.Data.PlanningTaskData.Interface;
using AutoPlannerApi.Data.PlanningTaskData.Model;
using AutoPlannerApi.Data.PlanningTaskData.Model.Answer;
using AutoPlannerApi.Data.PlanningTaskData.Model.Answer.AnswerStatus;

namespace AutoPlannerApi.Data.PlanningTaskData.Realization
{
    public class PlanningTaskRamRepository : IPlanningTaskDatabaseRepository
    {
        private List<PlanningTaskDatabase> _planningTasks = new List<PlanningTaskDatabase>();

        public Task<AddPlanningTaskDatabaseAnswer> Add(PlanningTaskDatabase planningTask)
        {
            _planningTasks.Add(planningTask);
            return Task.FromResult(new AddPlanningTaskDatabaseAnswer()
            {
                Status = new AddPlanningTaskDatabaseAnswerStatus() { Status = AddPlanningTaskDatabaseAnswerStatus.Good },
            });
        }

        public Task<DeletePlanningTaskDatabaseAnswer> DeleteByUserId(int userId)
        {
            var delete = new List<PlanningTaskDatabase>();
            foreach (var task in _planningTasks)
            {
                if (task.UserId == userId)
                {
                    delete.Add(task);
                }
            }
            foreach(var deleteTask in delete)
            {
                _planningTasks.Remove(deleteTask);
            }    
            return Task.FromResult(new DeletePlanningTaskDatabaseAnswer()
            {
                Status = new DeletePlanningTaskDatabaseAnswerStatus() { Status = DeletePlanningTaskDatabaseAnswerStatus.Good },
            });
        }

        public Task<GetPlanningTasksByUserIdDatabaseAnswer> Get(int userId)
        {
            var answer = new List<PlanningTaskDatabase>();
            foreach (var task in _planningTasks)
            {
                if (task.UserId == userId)
                {
                    answer.Add(task);
                }    
            }
            return Task.FromResult(new GetPlanningTasksByUserIdDatabaseAnswer()
            {
                Status = new GetPlanningTasksByUserIdDatabaseAnswerStatus() { Status = GetPlanningTasksByUserIdDatabaseAnswerStatus.Good },
                PlanningTasks = answer,
            });
        }
    }
}
