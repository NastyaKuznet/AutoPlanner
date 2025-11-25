using AutoPlannerApi.Data.Common.Model;
using AutoPlannerApi.Data.PlanningTaskData.Interface;
using AutoPlannerApi.Data.PlanningTaskData.Model.Answer.AnswerStatus;
using AutoPlannerApi.Data.TimeTableData.Interface;
using AutoPlannerApi.Data.TimeTableData.Model;
using AutoPlannerApi.Data.TimeTableData.Model.Answer;
using AutoPlannerApi.Data.TimeTableData.Model.Answer.AnswerStatus;
using AutoPlannerApi.Domain.TaskDomain.Interface;
using AutoPlannerApi.Domain.TaskDomain.Model;
using AutoPlannerApi.Domain.TaskDomain.Model.AnswerStatus;
using AutoPlannerApi.Domain.TimeTableDomain.Interface;
using AutoPlannerApi.Domain.TimeTableDomain.Model;
using AutoPlannerApi.Domain.TimeTableDomain.Model.Answer;
using AutoPlannerApi.Domain.TimeTableDomain.Model.Answer.AnswerStatus;
using AutoPlannerApi.Domain.UserDomain.Interface;
using AutoPlannerApi.Domain.UserDomain.Model.AnswerStatus;
using AutoPlannerApi.TelegramServices.Notifications;
using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Output.Model;
using AutoPlannerCore.Planning;
using Hangfire;
using System.Threading.Tasks;

namespace AutoPlannerApi.Domain.TimeTableDomain.Realization
{
    public class TimeTableItemClassicService : ITimeTableItemService
    {
        private ITimeTableItemDatabaseRepository _timeTableDatabaseRepository;
        private IUserService _userService;
        private ITaskService _taskService;
        private IPlanningTaskDatabaseRepository _planningTaskDatabaseRepository;


        public TimeTableItemClassicService(
            ITimeTableItemDatabaseRepository timeTableDatabaseRepository,
            IUserService userService,
            ITaskService taskService, 
            IPlanningTaskDatabaseRepository planningTaskDatabaseRepository)
        {
            _timeTableDatabaseRepository = timeTableDatabaseRepository;
            _userService = userService;
            _taskService = taskService;
            _planningTaskDatabaseRepository = planningTaskDatabaseRepository;
        }

        public async Task<(GetTTByUserIdAnswerStatusDomain, List<TimeTableItemDomain>, List<PlanningTaskDomain>)> Get(int userId)
        {
            var userExist = await _userService.Check(userId);
            if (userExist.Status == CheckAnswerStatusDomain.UserNotExists)
            {
                return ( 
                    new GetTTByUserIdAnswerStatusDomain() 
                    {
                        Status = GetTTByUserIdAnswerStatusDomain.UserNotExist
                    }, 
                    new List<TimeTableItemDomain>(),
                    new List<PlanningTaskDomain>());
            }
            var timeTableItemsAnswer = await _timeTableDatabaseRepository.Get(userId);
            if (timeTableItemsAnswer.Status.Status == ClassicAnswerStatus.Bad)
            {
                return (
                    new GetTTByUserIdAnswerStatusDomain()
                    {
                        Status = GetTTByUserIdAnswerStatusDomain.Bad
                    },
                    new List<TimeTableItemDomain>(),
                    new List<PlanningTaskDomain>());
            }
            var timeTableItemsDomain = new List<TimeTableItemDomain>();
            foreach(var timeTableItemDatabase in timeTableItemsAnswer.TimeTableItems)
            {
                timeTableItemsDomain.Add(new TimeTableItemDomain(
                    timeTableItemDatabase.MyTaskId,
                    timeTableItemDatabase.UserId,
                    timeTableItemDatabase.CountFrom,
                    timeTableItemDatabase.Name,
                    timeTableItemDatabase.Priority,
                    timeTableItemDatabase.StartDateTime,
                    timeTableItemDatabase.EndDateTime,
                    timeTableItemDatabase.IsComplete,
                    timeTableItemDatabase.CompleteDateTime));
            }
            var allPlanningTasksAnswer = await _planningTaskDatabaseRepository.Get(userId);
            if (allPlanningTasksAnswer.Status.Status == GetPlanningTasksByUserIdDatabaseAnswerStatus.Bad)
            {
                return (
                    new GetTTByUserIdAnswerStatusDomain()
                    {
                        Status = GetTTByUserIdAnswerStatusDomain.Bad
                    },
                    new List<TimeTableItemDomain>(),
                    new List<PlanningTaskDomain>());
            }
            var penaltyTasksDomain = new List<PlanningTaskDomain>();
            foreach (var task in allPlanningTasksAnswer.PlanningTasks)
            {
                penaltyTasksDomain.Add(new PlanningTaskDomain(
                    task.UserId,
                    task.MyTaskId,
                    task.Name,
                    task.Description,
                    task.Priority,
                    task.StartDateTime,
                    task.EndDateTime,
                    $"{task.Duration.Value.Days}:{task.Duration.Value.Hours}:{task.Duration.Value.Minutes}:{task.Duration.Value.Seconds}",
                    task.CountFrom,
                    task.IsComplete,
                    task.CompleteDateTime,
                    task.StartDateTimeRange,
                    task.EndDateTimeRange,
                    task.RuleOneTask,
                    task.StartDateTimeRuleOneTask,
                    task.EndDateTimeRuleOneTask,
                    task.RuleTwoTask,
                    task.TimePositionRegardingTaskId,
                    task.SecondTaskId,
                    task.RelationRangeId,
                    task.DateTimeRange));
            }
            return (
                new GetTTByUserIdAnswerStatusDomain()
                { 
                    Status = GetTTByUserIdAnswerStatusDomain.Good
                },
                timeTableItemsDomain,
                penaltyTasksDomain);
        }

        public async Task<RecreateTimeTableAnswerDomain> Recreate(int userId, DateTime startTimeTable, DateTime endDateTime)
        {
            // Проверка существования пользователя происходит в _taskService.Get
            var getTasksAnswer = await _taskService.Get(userId);
            var mapGetTaskStatusToRecreate = new Dictionary<int, int>() 
            {
                { GetTTByUserIdAnswerStatusDomain.Bad, RecreateTimeTableAnswerStatusDomain.Bad },
                { GetTTByUserIdAnswerStatusDomain.UserNotExist, RecreateTimeTableAnswerStatusDomain.UserNotExist },
            };
            if (getTasksAnswer.Status.Status != GetTTByUserIdAnswerStatusDomain.Good)
            {
                return new RecreateTimeTableAnswerDomain(
                    new RecreateTimeTableAnswerStatusDomain()
                    {
                        Status = mapGetTaskStatusToRecreate[getTasksAnswer.Status.Status],
                    },
                    new List<TimeTableItemDomain>(),
                    new List<PlanningTaskDomain>());
            }
            var getTimeTablesAnswer = await _timeTableDatabaseRepository.Get(userId);
            if (getTimeTablesAnswer.Status.Status == ClassicAnswerStatus.Bad)
            {
                return new RecreateTimeTableAnswerDomain(
                    new RecreateTimeTableAnswerStatusDomain()
                    {
                        Status = RecreateTimeTableAnswerStatusDomain.Bad,
                    },
                    new List<TimeTableItemDomain>(),
                    new List<PlanningTaskDomain>());
            }

            var tasks = new List<MyTask>(); // вообще везде бы классы конверторы поделать
            var secondTaskNeed = new List<TaskGetDomain>();
            foreach (var taskGet in getTasksAnswer.Tasks)
            {
                /*if (getTimeTablesAnswer.TimeTableItems.Count(x => x.MyTaskId == taskGet.Id) == 1)
                {
                    continue;
                }
                if (getTimeTablesAnswer.TimeTableItems.Count(x => x.MyTaskId == taskGet.Id) > 1)
                {
                    throw new Exception("WTF");
                }*/
                if (taskGet.RuleTwoTask)
                {
                    secondTaskNeed.Add(taskGet);
                }
                else {
                    tasks.Add(new MyTask()
                    {
                        Id = taskGet.Id,
                        Name = taskGet.Name,
                        Description = taskGet.Description,
                        CreatedDate = taskGet.CreatedDate,
                        Priority = taskGet.Priority,
                        StartDateTime = taskGet.StartDateTime,
                        EndDateTime = taskGet.EndDateTime,
                        Duration = TimeSpan.ParseExact(taskGet.Duration, @"dd\:hh\:mm\:ss", null),
                        IsRepit = taskGet.IsRepit,
                        RepitDateTime = taskGet.RepitTime,
                        IsRepitFromStart = taskGet.IsRepitFromStart,
                        CountRepit = taskGet.CountRepit,
                        StartDateTimeRepit = taskGet.StartDateTimeRepit,
                        EndDateTimeRepit = taskGet.EndDateTimeRepit,
                        RuleOneTask = taskGet.RuleOneTask ? new RuleOneTask()
                        {
                            Is = true,
                            StartDateTime = taskGet.StartDateTimeRuleOneTask is not null
                            ? (DateTime)taskGet.StartDateTimeRuleOneTask
                            : DateTime.Now, // вообще не гуд, но мало времени
                            EndDateTime = taskGet.EndDateTimeRuleOneTask is not null
                            ? (DateTime)taskGet.EndDateTimeRuleOneTask
                            : DateTime.Now,// вообще не гуд, но мало времени
                        } : null,
                        RuleTwoTask = taskGet.RuleTwoTask ? new RuleTwoTask()
                        {
                            TimePositionRegardingTask = (TimePosition)taskGet.TimePositionRegardingTaskId,
                            SecondTask = tasks.First(x => x.Id == taskGet.SecondTaskId),
                            RelationRange = (RelationRangeType)taskGet.RelationRangeId,
                            DateTimeRange = taskGet.DateTimeRange is not null
                            ? (TimeSpan)taskGet.DateTimeRange
                            : TimeSpan.Zero,// вообще не гуд, но мало времени
                        } : null,
                    });
                }
            }
            foreach (var t in secondTaskNeed)
            {
                if(tasks.Count(x => x.Id == t.SecondTaskId) == 0)
                {
                    continue;
                }
                tasks.Add(new MyTask()
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    CreatedDate = t.CreatedDate,
                    Priority = t.Priority,
                    StartDateTime = t.StartDateTime,
                    EndDateTime = t.EndDateTime,
                    Duration = TimeSpan.ParseExact(t.Duration, @"dd\:hh\:mm\:ss", null),
                    IsRepit = t.IsRepit,
                    RepitDateTime = t.RepitTime,
                    IsRepitFromStart = t.IsRepitFromStart,
                    CountRepit = t.CountRepit,
                    StartDateTimeRepit = t.StartDateTimeRepit,
                    EndDateTimeRepit = t.EndDateTimeRepit,
                    RuleOneTask = t.RuleOneTask ? new RuleOneTask()
                    {
                        Is = true,
                        StartDateTime = t.StartDateTimeRuleOneTask is not null
                            ? (DateTime)t.StartDateTimeRuleOneTask
                            : DateTime.Now, // вообще не гуд, но мало времени
                        EndDateTime = t.EndDateTimeRuleOneTask is not null
                            ? (DateTime)t.EndDateTimeRuleOneTask
                            : DateTime.Now,// вообще не гуд, но мало времени
                    } : null,
                    RuleTwoTask = t.RuleTwoTask ? new RuleTwoTask()
                    {
                        TimePositionRegardingTask = (TimePosition)t.TimePositionRegardingTaskId,
                        SecondTask = tasks.First(x => x.Id == t.SecondTaskId),
                        RelationRange = (RelationRangeType)t.RelationRangeId,
                        DateTimeRange = t.DateTimeRange is not null
                            ? (TimeSpan)t.DateTimeRange
                            : TimeSpan.Zero,// вообще не гуд, но мало времени
                    } : null,
                });
            }
            var timeTableItems = new List<TimeTableItem>();
            foreach (var timeTableItem in getTimeTablesAnswer.TimeTableItems)
            {
                timeTableItems.Add(new TimeTableItem() 
                {
                    MyTaskId = timeTableItem.MyTaskId,
                    CountFrom = timeTableItem.CountFrom,
                    Name = timeTableItem.Name,
                    StartDateTime = timeTableItem.StartDateTime,
                    EndDateTime = timeTableItem.EndDateTime,
                    IsComplete = timeTableItem.IsComplete,
                    CompleteDateTime = timeTableItem.CompleteDateTime,
                });
            }
            var table = new TimeTable() 
            {
            };

            var planner = new Planner(new PreparingTaskForPlanner(startTimeTable, endDateTime));
            planner.DoPlan(tasks, table);
            

            var afterTTI = new List<TimeTableItemDomain>();
            var flag = false;
            foreach (var forDelete in getTimeTablesAnswer.TimeTableItems)
            {
                await _timeTableDatabaseRepository.Delete(forDelete.MyTaskId);
            }
            foreach (var afterTimeTableItems in table.TimeTableItems) 
            {
                var maybeComplete = false;
                if (getTimeTablesAnswer.TimeTableItems.Count(x => x.MyTaskId == afterTimeTableItems.MyTaskId && x.CountFrom == afterTimeTableItems.CountFrom) != 0)
                {
                    maybeComplete = getTimeTablesAnswer.TimeTableItems.First(x => x.MyTaskId == afterTimeTableItems.MyTaskId && x.CountFrom == afterTimeTableItems.CountFrom).IsComplete;
                }
                    
                afterTTI.Add(new TimeTableItemDomain(
                    afterTimeTableItems.MyTaskId,
                    userId,
                    afterTimeTableItems.CountFrom,
                    afterTimeTableItems.Name,
                    afterTimeTableItems.Priority,
                    afterTimeTableItems.StartDateTime,
                    afterTimeTableItems.EndDateTime,
                    afterTimeTableItems.IsComplete || maybeComplete,
                    afterTimeTableItems.CompleteDateTime));

                var state = await _timeTableDatabaseRepository.Add(
                    new TimeTableItemForAddDatabase(
                        afterTimeTableItems.MyTaskId,
                        userId,
                        afterTimeTableItems.CountFrom,
                        afterTimeTableItems.Name,
                        afterTimeTableItems.Priority,
                        afterTimeTableItems.StartDateTime,
                        afterTimeTableItems.EndDateTime,
                        afterTimeTableItems.IsComplete || maybeComplete,
                        afterTimeTableItems.CompleteDateTime));
                if (state.Status != AddTimeTableItemAnswerStatusDatabase.Good)
                {
                    flag = true;
                }
            }


            var resultDelete = await _planningTaskDatabaseRepository.DeleteByUserId(userId);
            if (resultDelete.Status.Status == DeletePlanningTaskDatabaseAnswerStatus.Bad)
            {
                return new RecreateTimeTableAnswerDomain(
                new RecreateTimeTableAnswerStatusDomain()
                {
                    Status = RecreateTimeTableAnswerStatusDomain.Bad,
                },
                new List<TimeTableItemDomain>(),
                new List<PlanningTaskDomain>());
            }
            var afterPenaltyTasks = new List<PlanningTaskDomain>();
            foreach (var aftPenTask in planner.PenaltyTask)
            {
                var result = await _planningTaskDatabaseRepository.Add(new Data.PlanningTaskData.Model.PlanningTaskDatabase(
                    userId,
                    aftPenTask.MyTaskId,
                    aftPenTask.Name,
                    aftPenTask.Description,
                    aftPenTask.Priority,
                    aftPenTask.StartDateTime,
                    aftPenTask.EndDateTime,
                    aftPenTask.Duration,
                    aftPenTask.CountFrom,
                    aftPenTask.IsComplete,
                    aftPenTask.CompleteDateTime,
                    aftPenTask.StartDateTimeRange,
                    aftPenTask.EndDateTimeRange,
                    aftPenTask.RuleOneTask is not null,
                    aftPenTask.RuleOneTask is not null ? aftPenTask.RuleOneTask.StartDateTime : null,
                    aftPenTask.RuleOneTask is not null ? aftPenTask.RuleOneTask.EndDateTime : null,
                    aftPenTask.RuleTwoTask is not null,
                    aftPenTask.RuleTwoTask is not null ? (int)aftPenTask.RuleTwoTask.TimePositionRegardingTask : 0,
                    aftPenTask.RuleTwoTask is not null ? aftPenTask.RuleTwoTask.SecondTask.Id : 0,
                    aftPenTask.RuleTwoTask is not null ? (int)aftPenTask.RuleTwoTask.RelationRange : 0,
                    aftPenTask.RuleTwoTask is not null ? aftPenTask.RuleTwoTask.DateTimeRange : null));
                if (result.Status.Status == AddPlanningTaskDatabaseAnswerStatus.Bad)
                {
                    return new RecreateTimeTableAnswerDomain(
                    new RecreateTimeTableAnswerStatusDomain()
                    {
                        Status = RecreateTimeTableAnswerStatusDomain.Bad,
                    },
                    new List<TimeTableItemDomain>(),
                    new List<PlanningTaskDomain>());
                }
                afterPenaltyTasks.Add(new PlanningTaskDomain(
                    userId,
                    aftPenTask.MyTaskId,
                    aftPenTask.Name,
                    aftPenTask.Description,
                    aftPenTask.Priority,
                    aftPenTask.StartDateTime,
                    aftPenTask.EndDateTime,
                    $"{aftPenTask.Duration.Value.Days}:{aftPenTask.Duration.Value.Hours}:{aftPenTask.Duration.Value.Minutes}:{aftPenTask.Duration.Value.Seconds}",
                    aftPenTask.CountFrom,
                    aftPenTask.IsComplete,
                    aftPenTask.CompleteDateTime,
                    aftPenTask.StartDateTimeRange,
                    aftPenTask.EndDateTimeRange,
                    aftPenTask.RuleOneTask is not null,
                    aftPenTask.RuleOneTask is not null ? aftPenTask.RuleOneTask.StartDateTime : null,
                    aftPenTask.RuleOneTask is not null ? aftPenTask.RuleOneTask.EndDateTime : null,
                    aftPenTask.RuleTwoTask is not null,
                    aftPenTask.RuleTwoTask is not null ? (int) aftPenTask.RuleTwoTask.TimePositionRegardingTask : 0,
                    aftPenTask.RuleTwoTask is not null ? aftPenTask.RuleTwoTask.SecondTask.Id : 0,
                    aftPenTask.RuleTwoTask is not null ? (int)aftPenTask.RuleTwoTask.RelationRange :0,
                    aftPenTask.RuleTwoTask is not null ? aftPenTask.RuleTwoTask.DateTimeRange : null));
            }

            if (flag)
            {
                return new RecreateTimeTableAnswerDomain(
                    new RecreateTimeTableAnswerStatusDomain()
                    {
                        Status = RecreateTimeTableAnswerStatusDomain.NotFullLoadTask,
                    },
                    new List<TimeTableItemDomain>(),
                    new List<PlanningTaskDomain>());
            }

            BackgroundJob.Enqueue<INotificationSchedulerService>(x =>
                x.ScheduleUserNotificationsAsync(userId));

            return new RecreateTimeTableAnswerDomain(
                new RecreateTimeTableAnswerStatusDomain()
                {
                    Status = RecreateTimeTableAnswerStatusDomain.Good,
                },
                afterTTI,
                afterPenaltyTasks);
        }

        public async Task<DeleteItemFormTimeTableAnswerStatusDomain> Delete(int taskId)
        {
            var deleteStatus = await _timeTableDatabaseRepository.Delete(taskId);
            var mapDeleteDataToDomain = new Dictionary<int, int>() 
            {
                { DeleteTaskFromTimeTableAnswerStatusDatabase.Good, DeleteItemFormTimeTableAnswerStatusDomain.Good },
                { DeleteTaskFromTimeTableAnswerStatusDatabase.Bad, DeleteItemFormTimeTableAnswerStatusDomain.Bad },
                { DeleteTaskFromTimeTableAnswerStatusDatabase.TaskIsNotExist, DeleteItemFormTimeTableAnswerStatusDomain.TaskNotExist },
            };
            return new DeleteItemFormTimeTableAnswerStatusDomain()
            {
                Status = mapDeleteDataToDomain[deleteStatus.Status]
            };
        }
    }
}
