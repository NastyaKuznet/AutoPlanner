using AutoPlannerApi.Data.Common.Model;
using AutoPlannerApi.Data.TaskData.Interface;
using AutoPlannerApi.Data.TaskData.Model;
using AutoPlannerApi.Data.TaskData.Model.Answer;
using AutoPlannerApi.Data.TaskData.Model.Answer.AnswerStatus;

namespace AutoPlannerApi.Data.TaskData.Realization
{
    public class TaskRamRepository : ITaskDatabaseRepository
    {
        private List<TaskDatabase> _tasks = new List<TaskDatabase>();
        private int _tasksId = 1;
        public Task<AddTaskAnswerStatusData> Add(TaskForAddData taskForAdd, int userId)
        {
            // TODO add validation
            _tasks.Add(new TaskDatabase(
                _tasksId++,
                userId,
                taskForAdd.Name,
                taskForAdd.Description,
                taskForAdd.CreatedDate,
                taskForAdd.Priority,
                taskForAdd.StartDateTime,
                taskForAdd.EndDateTime,
                taskForAdd.Duration,
                taskForAdd.IsRepit,
                taskForAdd.RepitTime,
                taskForAdd.IsRepitFromStart,
                taskForAdd.CountRepit,
                taskForAdd.StartDateTimeRepit,
                taskForAdd.EndDateTimeRepit,
                taskForAdd.RuleOneTask,
                taskForAdd.StartDateTimeRuleOneTask,
                taskForAdd.EndDateTimeRuleOneTask,
                taskForAdd.RuleTwoTask,
                taskForAdd.TimePositionRegardingTaskId,
                taskForAdd.SecondTaskId,
                taskForAdd.RelationRangeId,
                taskForAdd.DateTimeRange,
                taskForAdd.IsComplete,
                taskForAdd.CompleteDateTime));
            return Task.FromResult(new AddTaskAnswerStatusData() { Status = AddTaskAnswerStatusData.Good });
        }

        public Task<DeleteTaskAnswerStatusDatabase> Delete(int taskId)
        {
            TaskDatabase deleteTask = null;
            foreach (var task in _tasks)
            {
                if (task.Id == taskId)
                {
                    deleteTask = task;
                }
            }
            if (deleteTask == null)
            {
                return Task.FromResult(new DeleteTaskAnswerStatusDatabase() { Status = DeleteTaskAnswerStatusDatabase.TaskNotExist });
            }
            _tasks.Remove(deleteTask);
            return Task.FromResult(new DeleteTaskAnswerStatusDatabase() { Status = DeleteTaskAnswerStatusDatabase.Good });
        }

        public Task<TaskForEditAnswerStatusData> Edit(TaskForEditDatabase taskForEdit)
        {
            TaskDatabase deleteTask = null;
            foreach (var task in _tasks)
            {
                if (task.Id == taskForEdit.Id)
                {
                    deleteTask = task;
                }
            }
            if (deleteTask == null)
            {
                return Task.FromResult(new TaskForEditAnswerStatusData() { Status = TaskForEditAnswerStatusData.TaskNotExist });
            }
            _tasks.Add(new TaskDatabase(
                taskForEdit.Id,
                deleteTask.UserId,
                taskForEdit.Name,
                taskForEdit.Description,
                deleteTask.CreatedDate,
                taskForEdit.Priority,
                taskForEdit.StartDateTime,
                taskForEdit.EndDateTime,
                taskForEdit.Duration,
                taskForEdit.IsRepit,
                taskForEdit.RepitTime,
                taskForEdit.IsRepitFromStart,
                taskForEdit.CountRepit,
                taskForEdit.StartDateTimeRepit,
                taskForEdit.EndDateTimeRepit,
                taskForEdit.RuleOneTask,
                taskForEdit.StartDateTimeRuleOneTask,
                taskForEdit.EndDateTimeRuleOneTask,
                taskForEdit.RuleTwoTask,
                taskForEdit.TimePositionRegardingTaskId,
                taskForEdit.SecondTaskId,
                taskForEdit.RelationRangeId,
                taskForEdit.DateTimeRange,
                taskForEdit.IsComplete,
                taskForEdit.CompleteDateTime));
            _tasks.Remove(deleteTask);
            return Task.FromResult(new TaskForEditAnswerStatusData() { Status = TaskForEditAnswerStatusData.Good });
        }

        public Task<GetByUserIdAnswerData> Get(int userId)
        {
            var userTasks = new List<TaskDatabase>();
            foreach(var task in _tasks)
            {
                if (task.UserId == userId)
                {
                    userTasks.Add(task);
                }
            }
            return Task.FromResult(new GetByUserIdAnswerData(
                new ClassicAnswerStatus() 
                { 
                    Status = ClassicAnswerStatus.Good
                },
                userTasks));
        }

        public Task<SetCompleteAnswerStatusDatabase> SetComplete(int taskId)
        {
            var flag = false;
            foreach (var task in _tasks)
            {
                if (task.Id == taskId)
                {
                    task.IsComplete = true;
                    task.CompleteDateTime = DateTime.Now;
                    flag = true;
                }
            }

            if (!flag)
            {
                return Task.FromResult(new SetCompleteAnswerStatusDatabase() { Status = SetCompleteAnswerStatusDatabase.TaskNotExist });
            }
            return Task.FromResult(new SetCompleteAnswerStatusDatabase() { Status = SetCompleteAnswerStatusDatabase.Good });
        }
    }
}
