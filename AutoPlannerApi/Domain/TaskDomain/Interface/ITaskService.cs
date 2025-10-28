using AutoPlannerApi.Domain.TaskDomain.Model;
using AutoPlannerApi.Domain.TaskDomain.Model.Answer;
using AutoPlannerApi.Domain.TaskDomain.Model.AnswerStatus;

namespace AutoPlannerApi.Domain.TaskDomain.Interface
{
    public interface ITaskService
    {
        public Task<AddTaskAnswerStatusDomain> Add(TaskForAddDomain taskForAdd, int userId);

        public Task<GetByUserIdAnswerDomain> Get(int userId);

        public Task<TaskForEditAnswerStatusDomain> Edit(TaskForEditDomain taskForEdit);

        public Task<DeleteTaskAnswerStatusDomain> Delete(int taskId);

        public Task<SetCompleteAnswerStatusDomain> SetComplete(int taskId);
    }
}
