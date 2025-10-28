using AutoPlannerApi.Data.Common.Model;
using AutoPlannerApi.Data.TaskData.Interface;
using AutoPlannerApi.Data.TaskData.Model;
using AutoPlannerApi.Data.TaskData.Model.Answer.AnswerStatus;
using AutoPlannerApi.Data.TimeTableData.Interface;
using AutoPlannerApi.Data.TimeTableData.Model.Answer.AnswerStatus;
using AutoPlannerApi.Domain.TaskDomain.Interface;
using AutoPlannerApi.Domain.TaskDomain.Model;
using AutoPlannerApi.Domain.TaskDomain.Model.Answer;
using AutoPlannerApi.Domain.TaskDomain.Model.AnswerStatus;
using AutoPlannerApi.Domain.UserDomain.Interface;
using AutoPlannerApi.Domain.UserDomain.Model.AnswerStatus;

namespace AutoPlannerApi.Domain.TaskDomain.Realization
{
    public class TaskClassicService : ITaskService
    {
        private IUserService _userService;
        private ITaskDatabaseRepository _taskDatabaseRepository;
        private ITimeTableItemDatabaseRepository _timeTableItemRepository;

        public TaskClassicService(ITaskDatabaseRepository taskDatabaseRepository, IUserService userService, ITimeTableItemDatabaseRepository timeTableItemRepository)
        {
            _taskDatabaseRepository = taskDatabaseRepository;
            _userService = userService;
            _timeTableItemRepository = timeTableItemRepository;
        }
        public async Task<AddTaskAnswerStatusDomain> Add(TaskForAddDomain taskForAdd, int userId)
        {
            // TODO more validation
            var checkUser = await _userService.Check(userId);
            var mapDomainCheckToAddTask = new Dictionary<int, int>() 
            {
                { CheckAnswerStatusDomain.Good, AddTaskAnswerStatusDomain.Good },
                { CheckAnswerStatusDomain.Bad, AddTaskAnswerStatusDomain.Bad },
                { CheckAnswerStatusDomain.UserNotExists, AddTaskAnswerStatusDomain.UserNotExists },
            };
            if (checkUser.Status != CheckAnswerStatusDomain.UserExist)
            {
                return new AddTaskAnswerStatusDomain() { Status = mapDomainCheckToAddTask[checkUser.Status] };
            }
            var timePositionRegardingTaskDictConverter = new Dictionary<TaskPositionDomain, int>()
            {
                { TaskPositionDomain.Before, (int)TimePositionData.Before },
                { TaskPositionDomain.After, (int)TimePositionData.After },
            };

            var status = await _taskDatabaseRepository.Add(new TaskForAddData(
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
                taskForAdd.CompleteDateTime
                ), userId);

            var statusDictConverter = new Dictionary<int, int>()
            {
                { AddTaskAnswerStatusData.Good, AddTaskAnswerStatusDomain.Good},
                { AddTaskAnswerStatusData.Bad, AddTaskAnswerStatusDomain.Bad},
            };
            return new AddTaskAnswerStatusDomain() { Status = statusDictConverter[status.Status] };
        }

        public async Task<DeleteTaskAnswerStatusDomain> Delete(int taskId)
        {
            var resultDelete = await _taskDatabaseRepository.Delete(taskId);

            var mapEditStatusDataToDomain = new Dictionary<int, int>()
            {
                { DeleteTaskAnswerStatusDatabase.Good, DeleteTaskAnswerStatusDomain.Good },
                { DeleteTaskAnswerStatusDatabase.Bad, DeleteTaskAnswerStatusDomain.Bad },
                { DeleteTaskAnswerStatusDatabase.TaskNotExist, DeleteTaskAnswerStatusDomain.TaskNotExist },
            };
            if (resultDelete.Status != DeleteTaskAnswerStatusDatabase.Good)
            {
                return new DeleteTaskAnswerStatusDomain()
                {
                    Status = mapEditStatusDataToDomain[resultDelete.Status],
                };
            }
            var resultDeleteFromTimeTable = await _timeTableItemRepository.Delete(taskId);
            var mapDeleteToDomain = new Dictionary<int, int>()
            {
                { DeleteTaskFromTimeTableAnswerStatusDatabase.Bad, TaskForEditAnswerStatusDomain.NotDeleteFromTimeTable },
                { DeleteTaskFromTimeTableAnswerStatusDatabase.TaskIsNotExist, TaskForEditAnswerStatusDomain.Good },
                { DeleteTaskFromTimeTableAnswerStatusDatabase.Good, TaskForEditAnswerStatusDomain.Good }
            };

            return new DeleteTaskAnswerStatusDomain()
            {
                Status = mapDeleteToDomain[resultDeleteFromTimeTable.Status],
            };
        }

        public async Task<TaskForEditAnswerStatusDomain> Edit(TaskForEditDomain taskForEdit)
        {
            var resultEdit = await _taskDatabaseRepository.Edit(new TaskForEditDatabase(
                taskForEdit.Id,
                taskForEdit.Name,
                taskForEdit.Description,
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
            var mapEditStatusDataToDomain = new Dictionary<int, int>() 
            {
                { TaskForEditAnswerStatusData.Good, TaskForEditAnswerStatusDomain.Good },
                { TaskForEditAnswerStatusData.Bad, TaskForEditAnswerStatusDomain.Bad },
                { TaskForEditAnswerStatusData.TaskNotExist, TaskForEditAnswerStatusDomain.TaskNotExist },
            };
            if (resultEdit.Status != TaskForEditAnswerStatusData.Good)
            {
                return new TaskForEditAnswerStatusDomain()
                {
                    Status = mapEditStatusDataToDomain[resultEdit.Status],
                };
            }
            var resultDeleteFromTimeTable = await _timeTableItemRepository.Delete(taskForEdit.Id);
            var mapDeleteToDomain = new Dictionary<int, int>() 
            {
                { DeleteTaskFromTimeTableAnswerStatusDatabase.Bad, TaskForEditAnswerStatusDomain.NotDeleteFromTimeTable },
                { DeleteTaskFromTimeTableAnswerStatusDatabase.TaskIsNotExist, TaskForEditAnswerStatusDomain.Good },
                { DeleteTaskFromTimeTableAnswerStatusDatabase.Good, TaskForEditAnswerStatusDomain.Good }
            };
            
            return new TaskForEditAnswerStatusDomain()
            {
                Status = mapDeleteToDomain[resultEdit.Status],
            };
        }

        public async Task<GetByUserIdAnswerDomain> Get(int userId)
        {
            var checkUser = await _userService.Check(userId);
            var mapDomainCheckToAddTask = new Dictionary<int, int>()
            {
                { CheckAnswerStatusDomain.Good, GetByUserIdAnswerStatusDomain.Good },
                { CheckAnswerStatusDomain.Bad, GetByUserIdAnswerStatusDomain.Bad },
                { CheckAnswerStatusDomain.UserNotExists, GetByUserIdAnswerStatusDomain.UserNotExist },
            };
            if (checkUser.Status != CheckAnswerStatusDomain.UserExist)
            {
                return new GetByUserIdAnswerDomain(
                    new GetByUserIdAnswerStatusDomain() 
                    { 
                        Status = mapDomainCheckToAddTask[checkUser.Status]
                    },
                    new List<TaskGetDomain>());
            }
            var tasksAnswer = await _taskDatabaseRepository.Get(userId);
            if (tasksAnswer.Status.Status == ClassicAnswerStatus.Bad)
            {
                return new GetByUserIdAnswerDomain(
                    new GetByUserIdAnswerStatusDomain()
                    { 
                        Status = ClassicAnswerStatus.Bad,
                    },
                    new List<TaskGetDomain>());
            }
            var userTasksDomain = new List<TaskGetDomain>();
            foreach (var task in tasksAnswer.Tasks)
            {
                userTasksDomain.Add(new TaskGetDomain(
                    task.Id,
                    task.UserId,
                    task.Name,
                    task.Description,
                    task.CreatedDate,
                    task.Priority,
                    task.StartDateTime,
                    task.EndDateTime,
                    task.Duration,
                    task.IsRepit,
                    task.RepitTime,
                    task.IsRepitFromStart,
                    task.CountRepit,
                    task.StartDateTimeRepit,
                    task.EndDateTimeRepit,
                    task.RuleOneTask,
                    task.StartDateTimeRuleOneTask,
                    task.EndDateTimeRuleOneTask,
                    task.RuleTwoTask,
                    task.TimePositionRegardingTaskId,
                    task.SecondTaskId,
                    task.RelationRangeId,
                    task.DateTimeRange,
                    task.IsComplete,
                    task.CompleteDateTime));
            }
            return new GetByUserIdAnswerDomain(
                    new GetByUserIdAnswerStatusDomain()
                    {
                        Status = ClassicAnswerStatus.Good,
                    },
                    userTasksDomain);
        }

        public async Task<SetCompleteAnswerStatusDomain> SetComplete(int taskId)
        {
            var status = await _taskDatabaseRepository.SetComplete(taskId);
            var map = new Dictionary<int, int>()
            {
                { SetCompleteAnswerStatusDatabase.Good, SetCompleteAnswerStatusDomain.Good },
                { SetCompleteAnswerStatusDatabase.Bad, SetCompleteAnswerStatusDomain.Bad },
                { SetCompleteAnswerStatusDatabase.TaskNotExist, SetCompleteAnswerStatusDomain.TaskNotExist },
            };
            if (status.Status != SetCompleteAnswerStatusDatabase.Good)
            {
                return new SetCompleteAnswerStatusDomain()
                {
                    Status = map[status.Status]
                };
            }
            var statusTTI = await _timeTableItemRepository.SetComplete(taskId);
            var map2 = new Dictionary<int, int>()
            {
                { SetCompleteTimeTableItemAnswerStatusDatabase.Good, SetCompleteAnswerStatusDomain.Good },
                { SetCompleteTimeTableItemAnswerStatusDatabase.Bad, SetCompleteAnswerStatusDomain.Bad },
                { SetCompleteTimeTableItemAnswerStatusDatabase.TimeTableItemNotExist, SetCompleteAnswerStatusDomain.TimeTableNotExist },
            };
            return new SetCompleteAnswerStatusDomain()
            {
                Status = map2[statusTTI.Status]
            };
        }
    }
}
