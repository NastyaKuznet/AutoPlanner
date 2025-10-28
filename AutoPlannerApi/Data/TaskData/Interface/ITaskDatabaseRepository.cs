using AutoPlannerApi.Data.TaskData.Model;
using AutoPlannerApi.Data.TaskData.Model.Answer;
using AutoPlannerApi.Data.TaskData.Model.Answer.AnswerStatus;

namespace AutoPlannerApi.Data.TaskData.Interface
{
    public interface ITaskDatabaseRepository
    {
        public Task<AddTaskAnswerStatusData> Add(TaskForAddData taskForAdd, int userId);

        public Task<GetByUserIdAnswerData> Get(int userId);

        public Task<TaskForEditAnswerStatusData> Edit(TaskForEditDatabase taskForEdit);

        public Task<DeleteTaskAnswerStatusDatabase> Delete(int taskId);

        public Task<SetCompleteAnswerStatusDatabase> SetComplete(int taskId);
    }
}
