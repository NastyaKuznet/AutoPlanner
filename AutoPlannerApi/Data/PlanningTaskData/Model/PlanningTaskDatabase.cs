namespace AutoPlannerApi.Data.PlanningTaskData.Model
{
    public class PlanningTaskDatabase
    {
        public int UserId { get; set; }
        /// <summary>
        /// Уникальный индетификатор родительской задачи.
        /// </summary>
        public int MyTaskId { get; }

        /// <summary>
        /// Названия элемента.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Описение задачи.
        /// </summary>
        public string Description { get; }

        /// <inheritdoc/>
        public int Priority { get; }

        /// <summary>
        /// Дата и время начала задачи.
        /// </summary>
        public DateTime? StartDateTime { get; }

        /// <summary>
        /// Дата и время окончания задачи.
        /// </summary>
        public DateTime? EndDateTime { get; }

        /// <summary>
        /// Длительность задачи, может быть null.
        /// </summary>
        public TimeSpan? Duration { get; }

        /// <summary>
        /// Номер задачи из обшего множества повторяемой задачи.
        /// </summary>
        public int CountFrom { get; }

        /// <summary>
        /// Флаг, true - если задача выполнена, false - если задача не выполнена.
        /// </summary>
        public bool IsComplete { get; }

        /// <summary>
        /// Дата и время выполнения задачи.
        /// </summary>
        public DateTime? CompleteDateTime { get; }

        /// <summary>
        /// Начала диапазона даты и времени для установки задачи.
        /// </summary>
        public DateTime? StartDateTimeRange { get; }

        /// <summary>
        /// Крнец дипазона даты и времени для установки задачи.
        /// </summary>
        public DateTime? EndDateTimeRange { get; }

        /// <summary>
        /// Есть ли правило о допустимом диапазоне существования.
        /// </summary>
        public bool RuleOneTask { get; }

        /// <summary>
        /// Дата и время начала периода.
        /// </summary>
        public DateTime? StartDateTimeRuleOneTask { get; }

        /// <summary>
        /// Дата и время окончания периода.
        /// </summary>
        public DateTime? EndDateTimeRuleOneTask { get; }

        /// <summary>
        /// Есть ли правило зависимости от других задач.
        /// </summary>
        public bool RuleTwoTask { get; }

        /// <summary>
        /// Идентификатор временной позиция относительно задачи.
        /// </summary>
        public int TimePositionRegardingTaskId { get; }

        /// <summary>
        /// Идентификатор второй задачи, с которой связано правило.
        /// </summary>
        public int SecondTaskId { get; }

        /// <summary>
        /// Идентификатор отношения времени.
        /// </summary>
        public int RelationRangeId { get; }

        /// <summary>
        /// Диапазон времени между задачами.
        /// </summary>
        public TimeSpan? DateTimeRange { get; }

        public PlanningTaskDatabase(
            int userId,
            int myTaskId,
            string name,
            string description,
            int priority,
            DateTime? startDateTime,
            DateTime? endDateTime,
            TimeSpan? duration,
            int countFrom,
            bool isComplete,
            DateTime? completeDateTime,
            DateTime? startDateTimeRange,
            DateTime? endDateTimeRange,
            bool ruleOneTask,
            DateTime? startDateTimeRuleOneTask,
            DateTime? endDateTimeRuleOneTask,
            bool ruleTwoTask,
            int timePositionRegardingTaskId,
            int secondTaskId,
            int relationRangeId,
            TimeSpan? dateTimeRange)
        {
            UserId = userId;
            MyTaskId = myTaskId;
            Name = name;
            Description = description;
            Priority = priority;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Duration = duration;
            CountFrom = countFrom;
            IsComplete = isComplete;
            CompleteDateTime = completeDateTime;
            StartDateTimeRange = startDateTimeRange;
            EndDateTimeRange = endDateTimeRange;
            RuleOneTask = ruleOneTask;
            StartDateTimeRuleOneTask = startDateTimeRuleOneTask;
            EndDateTimeRuleOneTask = endDateTimeRuleOneTask;
            RuleTwoTask = ruleTwoTask;
            TimePositionRegardingTaskId = timePositionRegardingTaskId;
            SecondTaskId = secondTaskId;
            RelationRangeId = relationRangeId;
            DateTimeRange = dateTimeRange;
        }
    }
}
