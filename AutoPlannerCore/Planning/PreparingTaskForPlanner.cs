using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Output.Model;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Planning
{
    /// <summary>
    /// Класс для подготовки задач для <see cref="Planner"/> при составлении расписания.
    /// </summary>
    public class PreparingTaskForPlanner
    {
        private readonly DateTime _tableStartDate;
        private readonly DateTime _tableEndDate;

        public PreparingTaskForPlanner(
            DateTime tableStartDate, 
            DateTime tableEndDate)
        {
            _tableStartDate = tableStartDate;
            _tableEndDate = tableEndDate;
        }

        /// <summary>
        /// Преобразовать все задачи из <see cref="MyTask"/> в <see cref="PlanningTask"/>.
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public List<PlanningTask> Prepare(IReadOnlyCollection<MyTask> tasks)
        {
            var planningTasks = new List<PlanningTask>();
            foreach (var task in tasks)
            {
                if (task.IsRepit)
                {
                    if (task.StartDateTimeRepit == null)
                    {
                        task.StartDateTimeRepit = task.StartDateTime;
                        task.EndDateTimeRepit = task.StartDateTime + (task.Duration + task.RepitDateTime) * (task.CountRepit - 1) + task.Duration;
                    }
                    if (task.StartDateTimeRepit != null && task.CountRepit == 0)
                    {
                        var a = ((DateTime)task.EndDateTimeRepit - (TimeSpan)task.Duration - (DateTime)task.StartDateTimeRepit) / ((TimeSpan)task.Duration + (TimeSpan)task.RepitDateTime) + 1;
                        task.CountRepit = (int)Math.Round(a);
                    }
                    if (task.StartDateTimeRepit <= _tableEndDate && task.EndDateTimeRepit >= _tableStartDate)
                    {
                        var repitTasks = RepitTaskParser.Parse(task);
                        foreach (var repitTask in repitTasks)
                        {
                            if (repitTask.StartDateTime <= _tableEndDate &&
                                repitTask.EndDateTime >= _tableStartDate)
                            {
                                planningTasks.Add(repitTask);
                            }
                        }
                    }
                }
                else
                {
                    if (task.StartDateTime != null &&
                        task.StartDateTime <= _tableEndDate &&
                        task.EndDateTime >= _tableStartDate|| 
                        task.StartDateTime != null &&
                        task.StartDateTime <= _tableEndDate &&
                        task.StartDateTime + task.Duration >= _tableStartDate ||
                        task.StartDateTime == null)
                    {
                        planningTasks.Add(new PlanningTask()
                        {
                            MyTaskId = task.Id,
                            Name = task.Name,
                            Description = task.Description,
                            StartDateTime = task.StartDateTime,
                            EndDateTime = task.EndDateTime is null && task.StartDateTime is not null && task.Duration is not null ? task.StartDateTime + task.Duration : task.EndDateTime,
                            Priority = task.Priority,
                            RuleOneTask = task.RuleOneTask,
                            RuleTwoTask = task.RuleTwoTask,
                            Duration = task.Duration,
                            IsComplete = task.IsComplete,
                            CompleteDateTime = task.CompleteDateTime,
                        });
                    }
                }
            }
            return planningTasks;
        }

        public static void SetDateTimeFromRuleTwoTask(PlanningTask task, TimeTableItem secondTaskInTable)
        {
            if (task.RuleTwoTask is null)
            {
                throw new ArgumentException("Задача должна иметь правило RuleTwoTask.");
            }
            if (task.RuleTwoTask.TimePositionRegardingTask is TimePosition.After)
            {
                task.StartDateTime = secondTaskInTable.EndDateTime + task.RuleTwoTask.DateTimeRange;
                task.EndDateTime = task.StartDateTime + task.Duration;
            }
            else if (task.RuleTwoTask.TimePositionRegardingTask is TimePosition.Before)
            {
                task.EndDateTime = secondTaskInTable.StartDateTime - task.RuleTwoTask.DateTimeRange;
                task.StartDateTime = task.EndDateTime - task.Duration;
            }
        }

        public static void SetDateTimeRangeFromRuleOneTask(PlanningTask task)
        {
            if (task.RuleOneTask is null)
            {
                throw new ArgumentException("Задача должна иметь правило RuleOneTask.");
            }
            if (task.RuleOneTask.Is)
            {
                task.StartDateTimeRange = task.RuleOneTask.StartDateTime;
                task.EndDateTimeRange = task.RuleOneTask.EndDateTime;
            }
            else
            {
                // TODO
            }
        }

        public void SetDateTimeRangeFromRuleTwoTask(PlanningTask task, TimeTableItem secondTaskInTable)
        {
            if (task.RuleTwoTask is null)
            {
                throw new ArgumentException("Задача должна иметь правило RuleTwoTask.");
            }
            if (task.RuleTwoTask.RelationRange == RelationRangeType.Greater && task.RuleTwoTask.TimePositionRegardingTask == TimePosition.Before)
            {
                task.StartDateTimeRange = _tableStartDate;
                task.EndDateTimeRange = secondTaskInTable.StartDateTime - task.RuleTwoTask.DateTimeRange;
            }
            else if (task.RuleTwoTask.RelationRange == RelationRangeType.Less && task.RuleTwoTask.TimePositionRegardingTask == TimePosition.Before)
            {
                task.StartDateTimeRange = secondTaskInTable.StartDateTime - task.RuleTwoTask.DateTimeRange;
                task.EndDateTimeRange = secondTaskInTable.StartDateTime;
            }
            else if (task.RuleTwoTask.RelationRange == RelationRangeType.Greater && task.RuleTwoTask.TimePositionRegardingTask == TimePosition.After)
            {
                task.StartDateTimeRange = secondTaskInTable.EndDateTime + task.RuleTwoTask.DateTimeRange;
                task.EndDateTimeRange = _tableEndDate;
            }
            else if (task.RuleTwoTask.RelationRange == RelationRangeType.Less && task.RuleTwoTask.TimePositionRegardingTask == TimePosition.After)
            {
                task.StartDateTimeRange = secondTaskInTable.EndDateTime;
                task.EndDateTimeRange = secondTaskInTable.EndDateTime + task.RuleTwoTask.DateTimeRange + task.Duration;
            }
        }

        public void SetBaseDateTimeRange(PlanningTask task)
        {
            task.StartDateTimeRange = _tableStartDate;
            task.EndDateTimeRange = _tableEndDate;
        }
    }
}
