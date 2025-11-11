using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Output.Model;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Planning
{
    /// <summary>
    /// Формирователь расписания.
    /// </summary>
    public class Planner
    {
        private readonly PreparingTaskForPlanner _preparingTaskForPlanner;

        private List<PlanningTask> _pocket = new List<PlanningTask>();

        /// <summary>
        /// Задачи, которые не удалось поместить в расписание.
        /// </summary>
        public List<PlanningTask> PenaltyTask { get; private set; } = new List<PlanningTask> { };

        /// <summary>
        /// Вторичная очередь.
        /// </summary>
        public List<PlanningTask> SecondaryQueue = new List<PlanningTask>();

        /// <summary>
        /// Третичная очередь.
        /// </summary>
        public List<PlanningTask> TertiaryQueue = new List<PlanningTask>();

        public List<PlanningTask> QuaternaryQueue = new List<PlanningTask>();

        public Planner(PreparingTaskForPlanner preparingTask)
        {
            _preparingTaskForPlanner = preparingTask;
        }

        /// <summary>
        /// Изменить расписание.
        /// </summary>
        /// <param name="tasks">Задачи, которые нужно поместить в расписание.</param>
        /// <param name="table">Расписание в которое помещаются задачи.</param>
        /// <returns>Расписание.</returns>
        public void DoPlan(IReadOnlyCollection<MyTask> tasks, TimeTable table)
        {
            var planningTasks = _preparingTaskForPlanner.Prepare(tasks);
            // TODO сохранение в бд? ну не прям тут, в отдельном классе для атомарности, это просто заметка
            var forPrimaryCheckTasks = new List<PlanningTask>();
            planningTasks.ForEach(planningTask => { forPrimaryCheckTasks.Add(planningTask); });
            PrimaryCheck(forPrimaryCheckTasks, table, planningTasks);
            SecondaryCheck(table, planningTasks);
            TertiaryCheck(table, planningTasks);
        }

        public void PrimaryCheck(List<PlanningTask> planningTasks, TimeTable table, IReadOnlyCollection<PlanningTask> allPlanningTask)
        {
            foreach(var task in planningTasks)
            {
                if (table.TimeTableItems.Count(x => x.MyTaskId == task.MyTaskId && x.CountFrom == task.CountFrom) != 0)
                {
                    continue;
                }
                if (CheckStartDateTime(task))
                {
                    PlanningInStartTime(table, task, allPlanningTask);
                }
                else
                {
                    if (task.RuleOneTask is null &&  task.RuleTwoTask is null)
                    {
                        _preparingTaskForPlanner.SetBaseDateTimeRange(task);
                        TertiaryQueue.Add(task);

                    }
                    if (task.RuleOneTask is not null)
                    {
                        PreparingTaskForPlanner.SetDateTimeRangeFromRuleOneTask(task);
                        TertiaryQueue.Add(task);
                    }
                    if (task.RuleTwoTask is not null)
                    {
                        if (CheckSecondTaskInTimeTable(table, task))
                        {
                            if (task.RuleTwoTask.RelationRange is RelationRangeType.Equal)
                            {
                                PreparingTaskForPlanner.SetDateTimeFromRuleTwoTask(task, table.TimeTableItems.First(i => i.MyTaskId == task.RuleTwoTask.SecondTask.Id));
                                PlanningInStartTime(table, task, allPlanningTask);
                            }
                            else
                            {
                                _preparingTaskForPlanner.SetDateTimeRangeFromRuleTwoTask(task, table.TimeTableItems.First(i => i.MyTaskId == task.RuleTwoTask.SecondTask.Id));
                                TertiaryQueue.Add(task);
                            }
                        }
                        else
                        {
                            if (allPlanningTask.First(x => x.MyTaskId == task.RuleTwoTask.SecondTask.Id).StartDateTimeRange is not null)
                            {
                                QuaternaryQueue.Add(task);
                            }
                            else 
                            { 
                                SecondaryQueue.Add(task);
                            }
                        }
                    }
                }
            }
        }

        public void SecondaryCheck(TimeTable table, IReadOnlyCollection<PlanningTask> allPlanningTask)
        {
            while (SecondaryQueue.Count != 0)
            {
                var forSecondaryCheck = new List<PlanningTask>();
                SecondaryQueue.ForEach(item => { forSecondaryCheck.Add(item); });
                SecondaryQueue.Clear();
                PrimaryCheck(forSecondaryCheck, table, allPlanningTask);
            }
        }

        public void TertiaryCheck(TimeTable table, IReadOnlyCollection<PlanningTask> allPlanningTask)
        {
            var tertiaryQueue = TertiaryQueue.OrderByDescending(x => x.Priority).ThenByDescending(x => x.Duration);
            foreach (var task in tertiaryQueue)
            {
                var checkStart = (DateTime)task.StartDateTimeRange;
                var checkEnd = (DateTime)(checkStart + task.Duration);

                CheckEndIsNotEndRangeTet(table, checkStart, checkEnd, task, allPlanningTask);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkEnd"></param>
        /// <param name="task"></param>
        /// <returns>(NeedNext, CheckStartTime, CheckEndTime)</returns>
        public void CheckEndIsNotEndRangeTet(
            TimeTable table, 
            DateTime checkStart, 
            DateTime checkEnd, 
            PlanningTask task, 
            IReadOnlyCollection<PlanningTask> allPlanningTask)
        {
            if (CheckEndIsNotEndRange(checkEnd, task))
            {
                CheckFreeTet(table, checkStart, checkEnd, task, allPlanningTask);
            }
            else
            {
                PenaltyTask.Add(task);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="checkStart"></param>
        /// <param name="checkEnd"></param>
        /// <param name="task"></param>
        /// <returns>NeedNext</returns>
        public void CheckFreeTet(
            TimeTable table, 
            DateTime checkStart, 
            DateTime checkEnd, 
            PlanningTask task, 
            IReadOnlyCollection<PlanningTask> allPlanningTask)
        {
            if (CheckFree(table, checkStart, checkEnd))
            {
                task.StartDateTime = checkStart;
                task.EndDateTime = checkEnd;
                PutTaskInTimeTable(table, task);
                if (_pocket.Count != 0)
                {
                    PutPocketIntoPenaltyTask();
                    _pocket.Clear();
                }
            }
            else
            {
                var findTask = FindTaskTakeUpPlace(table, checkStart, checkEnd, allPlanningTask)
                                ?? throw new Exception("[Ошибка] Не смотря на то, что место не было свободно, не была найдена задача.");
                CheckTaskIsMoveableTet(table, task, findTask, checkStart, checkEnd, allPlanningTask);
            }
        }

        public void CheckTaskIsMoveableTet(
            TimeTable table, 
            PlanningTask baseTask, 
            PlanningTask findTask, 
            DateTime checkStart, 
            DateTime checkEnd, 
            IReadOnlyCollection<PlanningTask> allPlanningTask)
        {
            if (CheckTaskIsMoveable(findTask))
            {
                CanMoveMoveableTask(baseTask, checkStart, checkEnd, findTask, table, allPlanningTask);
            }
            else
            {
                CheckHighPriorityMoveableTaskTet(table, baseTask, findTask, checkStart, checkEnd, allPlanningTask);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="baseTask"></param>
        /// <param name="findTask"></param>
        /// <returns>(TryAgainCheckFree, CheckStartTime, CheckEndTime)</returns>
        public void CheckHighPriorityMoveableTaskTet(TimeTable table,
            PlanningTask baseTask,
            PlanningTask findTask,
            DateTime checkStart,
            DateTime checkEnd,
            IReadOnlyCollection<PlanningTask> allPlanningTask)
        {
            if (CheckHighPriority(baseTask, findTask))
            {
                PutTaskIntoPocketFromTable(findTask, table);
                CheckFreeTet(table, checkStart, checkEnd, baseTask, allPlanningTask);
            }
            else
            {
                if (_pocket.Count != 0)
                {
                    PutPocketIntoTable(table);
                    _pocket.Clear();
                }
                var newCheckStart = (DateTime)findTask.EndDateTime;
                var newCheckEnd = (DateTime)(newCheckStart + findTask.Duration);

                CheckEndIsNotEndRangeTet(table, newCheckStart, newCheckEnd, baseTask, allPlanningTask);
            }
        }

        public void CanMoveMoveableTask(
            PlanningTask baseTask, 
            DateTime checkStart, 
            DateTime checkEnd, 
            PlanningTask moveableTask,
            TimeTable table,
            IReadOnlyCollection<PlanningTask> allPlanningTask)
        {
            var moveTime = new TimeSpan();
            var moveStartTime = new DateTime();
            var moveEndTime = new DateTime();
            if (table.TimeTableItems.Count(x => x.MyTaskId == moveableTask.MyTaskId) == 1)
            {
                var tti = table.TimeTableItems.First(x => x.MyTaskId == moveableTask.MyTaskId);
                moveTime = (TimeSpan)baseTask.Duration + (checkStart - tti.StartDateTime);
                moveStartTime = (DateTime)(tti.StartDateTime + moveTime);
                moveEndTime = (DateTime)(moveStartTime + baseTask.Duration);

                // костыли мдэ
                moveableTask.StartDateTimeRange = moveableTask.RuleOneTask.StartDateTime;
                moveableTask.EndDateTimeRange = moveableTask.RuleOneTask.EndDateTime;
            }
            else
            {
                moveTime = (TimeSpan)baseTask.Duration + (checkStart - (DateTime)moveableTask.StartDateTime);
                moveStartTime = (DateTime)(moveableTask.StartDateTime + moveTime);
                moveEndTime = (DateTime)(moveStartTime + baseTask.Duration);
            }
            if (CheckEndIsNotEndRange(moveEndTime, moveableTask))
            {
                PutOutTaskInTimeTable(table, moveableTask);
                if (CheckFree(table, moveStartTime, moveEndTime))
                {
                    moveableTask.StartDateTime = moveStartTime;
                    moveableTask.EndDateTime = moveEndTime;
                    PutTaskInTimeTable(table, moveableTask);
                    baseTask.StartDateTime = checkStart;
                    baseTask.EndDateTime = checkEnd;
                    PutTaskInTimeTable(table, baseTask);
                    foreach (var task in _pocket)
                    {
                        if (task.MyTaskId != baseTask.MyTaskId)
                        {
                            PutTaskInTimeTable(table, task);
                        }
                    }
                    _pocket.Clear();
                }
                else
                {
                    var findTask = FindTaskTakeUpPlace(table, moveStartTime, moveEndTime, allPlanningTask)
                                ?? throw new Exception("[Ошибка] Не смотря на то, что место не было свободно, не была найдена задача.");
                    baseTask.StartDateTime = checkStart;
                    baseTask.EndDateTime = checkEnd;
                    _pocket.Add(baseTask);
                    CheckTaskIsMoveableTet(table, moveableTask, findTask, moveStartTime, moveEndTime, allPlanningTask);
                }
            }
            else
            {
                CheckHighPriorityMoveableTaskTet(table, baseTask, moveableTask, checkStart, checkEnd, allPlanningTask);
            }
        }

        public static bool CheckEndIsNotEndRange(DateTime checkEnd, PlanningTask task)
        {
            return checkEnd <= task.EndDateTimeRange;
        }

        public static bool CheckTaskIsMoveable(PlanningTask task)
        {
            return task.StartDateTimeRange is not null || task.RuleOneTask is not null;
        }

        private void PlanningInStartTime(TimeTable table, PlanningTask task, IReadOnlyCollection<PlanningTask> planningTasks)
        {
            while (true)
            {
                if (CheckFree(table, task))
                {
                    PutTaskInTimeTable(table, task);
                    if (_pocket.Count == 0)
                    {
                        break;
                    }
                    PutPocketIntoPenaltyTask();
                    _pocket.Clear();
                    break;
                }
                else
                {
                    var findTask = FindTaskTakeUpPlace(table, task, planningTasks)
                        ?? throw new Exception("[Ошибка] Не смотря на то, что место не было свободно, не была найдена задача.");
                    if (CheckHighPriority(task, findTask) && !findTask.IsComplete)
                    {
                        PutTaskIntoPocketFromTable(findTask, table);
                        continue;
                    }
                    else
                    {
                        PenaltyTask.Add(task);
                        if (_pocket.Count == 0)
                        {
                            break;
                        }
                        PutPocketIntoTable(table);
                        _pocket.Clear();
                        break;
                    }
                }
            }
        }

        private static bool CheckStartDateTime(PlanningTask task)
        {
            return task.StartDateTime != null;
        }

        private static bool CheckHighPriority(PlanningTask baseTask, PlanningTask findTask)
        {
            return baseTask.Priority > findTask.Priority;
        }

        /// <summary>
        /// Проверить что задачу можно поставить в расписание.
        /// </summary>
        /// <param name="table">Расписание, в которое предполагается поставить задачу.</param>
        /// <param name="task">Задачу, которую предполагается поставить в расписание.</param>
        /// <returns>True - задачу, можно поставить в расписание. False - задачу, нельзя поместить в расписание.</returns>
        public static bool CheckFree(TimeTable table, PlanningTask task)
        {
            foreach (var item in table.TimeTableItems)
            {
                return !(task.StartDateTime < item.EndDateTime && task.EndDateTime > item.StartDateTime);
            }
            return true;
        }

        /// <summary>
        /// Проверить что задачу можно поставить в расписание.
        /// </summary>
        /// <param name="table">Расписание, в которое предполагается поставить задачу.</param>
        /// <param name="startDateTime">Время начала задачи, которую предполагается поставить в расписание.</param>
        /// <param name="endDateTime">Время начала задачи, которую предполагается поставить в расписание.</param>
        /// <returns>True - задачу, можно поставить в расписание. False - задачу, нельзя поместить в расписание.</returns>
        public static bool CheckFree(TimeTable table, DateTime startDateTime, DateTime endDateTime)
        {
            foreach (var item in table.TimeTableItems)
            {
                if (startDateTime < item.EndDateTime && endDateTime > item.StartDateTime)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Найти задачу, которая занимает место в расписании для другой задачи.
        /// </summary>
        /// <param name="table">Расписание,в котором ведется поиск.</param>
        /// <param name="task">Задача,которую предполагали поставить в расписание.</param>
        /// <param name="tasks">Все задачи участвовующие в системе.</param>
        /// <returns>Задача, которая занимает место в расписании для другой задачи. Null, если задача не найдена.</returns>
        public static PlanningTask? FindTaskTakeUpPlace(TimeTable table, PlanningTask task, IReadOnlyCollection<PlanningTask> tasks)
        {
            foreach (var item in table.TimeTableItems)
            {
                if (task.StartDateTime < item.EndDateTime && task.EndDateTime > item.StartDateTime)
                {
                    return tasks.FirstOrDefault(t => t.MyTaskId == item.MyTaskId && t.CountFrom == item.CountFrom);
                }
            }
            return null;
        }

        /// <summary>
        /// Найти задачу, которая занимает место в расписании для другой задачи.
        /// </summary>
        /// <param name="table">Расписание,в котором ведется поиск.</param>
        /// <param name="tasks">Все задачи участвовующие в системе.</param>
        /// <returns>Задача, которая занимает место в расписании для другой задачи. Null, если задача не найдена.</returns>
        public static PlanningTask? FindTaskTakeUpPlace(TimeTable table, DateTime chechStart, DateTime checkEnd, IReadOnlyCollection<PlanningTask> tasks)
        {
            foreach (var item in table.TimeTableItems)
            {
                if (chechStart < item.EndDateTime && checkEnd > item.StartDateTime)
                {
                    return tasks.FirstOrDefault(t => t.MyTaskId == item.MyTaskId && t.CountFrom == item.CountFrom);
                }
            }
            return null;
        }

        /// <summary>
        /// Помешает задачу в расписание. 
        /// Предполагается, что до этого выполнен метод <see cref="CheckFree(TimeTable, MyTask)"/>.
        /// Также предполагается, что <see cref="MyTask.StartTime"/>, <see cref="MyTask.EndTime"/> и <see cref="MyTask.StartDay"/> не равны null.
        /// </summary>
        /// <param name="table">Расписание, в которое помещается задача.</param>
        /// <param name="task">Задача, которая помещается в расписание.ы</param>
        public static void PutTaskInTimeTable(TimeTable table, PlanningTask task)
        {
            var timeTableItem = new TimeTableItem()
            {
                MyTaskId = task.MyTaskId,
                CountFrom = task.CountFrom,
                Name = task.Name,
                Priority = task.Priority,
                StartDateTime = (DateTime)task.StartDateTime,
                EndDateTime = (DateTime)task.EndDateTime,
            };
            table.TimeTableItems.Add(timeTableItem);
        }

        /// <summary>
        /// Вынимает задачу из расписания. 
        /// Предполагается, что до этого выполнен метод <see cref="CheckFree(TimeTable, MyTask)"/>.
        /// Также предполагается, что <see cref="MyTask.StartTime"/>, <see cref="MyTask.EndTime"/> и <see cref="MyTask.StartDay"/> не равны null.
        /// </summary>
        /// <param name="table">Расписание, из которой вынимается задача.</param>
        /// <param name="task">Задача, из которой вынимается в расписание.</param>
        public static void PutOutTaskInTimeTable(TimeTable table, PlanningTask task)
        {
            table.TimeTableItems.Remove(table.TimeTableItems.First(x => x.MyTaskId == task.MyTaskId));
        }

        /// <summary>
        /// Помешает задачу в расписание. 
        /// Предполагается, что до этого выполнен метод <see cref="CheckFree(TimeTable, MyTask)"/>.
        /// Также предполагается, что <see cref="MyTask.StartTime"/>, <see cref="MyTask.EndTime"/> и <see cref="MyTask.StartDay"/> не равны null.
        /// </summary>
        /// <param name="table">Расписание, в которое помещается задача.</param>
        /// <param name="task">Задача, которая помещается в расписание.</param>
        /// <param name="startDateTime">Время начала задачи.</param>
        /// <param name="endDateTime">Время окончания задачи.</param>
        public static void PutTaskInTimeTable(TimeTable table, PlanningTask task, DateTime startDateTime, DateTime endDateTime)
        {
            var timeTableItem = new TimeTableItem()
            {
                MyTaskId = task.MyTaskId,
                CountFrom = task.CountFrom,
                Name = task.Name,
                Priority = task.Priority,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
            };
            table.TimeTableItems.Add(timeTableItem);
        }

        private void PutPocketIntoPenaltyTask()
        {
            foreach (var task in _pocket)
            {
                PenaltyTask.Add(task);
            }
            _pocket.Clear();
        }

        public void PutPocketIntoTable(TimeTable table)
        {
            foreach(var task in _pocket)
            {
                PutTaskInTimeTable(table, task);
            }
        }

        private void PutTaskIntoPocketFromTable(PlanningTask task, TimeTable table)
        {
            _pocket.Add(task);
            var item = table.TimeTableItems.First(i => i.MyTaskId == task.MyTaskId && i.CountFrom == task.CountFrom);
            table.TimeTableItems.Remove(item);
        }

        public static bool CheckSecondTaskInTimeTable(TimeTable timeTable, PlanningTask task)
        {
            return timeTable.TimeTableItems.Any(i => i.MyTaskId == task.RuleTwoTask.SecondTask.Id);
        }
    }
}
