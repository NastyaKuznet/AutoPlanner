namespace AutoPlannerApi.Data.TaskData.Model
{
    public class TaskDatabase
    {
        public int Id { get; }

        public int UserId { get; }
        public string Name { get; }

        public string Description { get; }

        public DateTime CreatedDate { get; } = DateTime.Now;

        public int Priority { get; } = 0;

        /// <summary>
        /// Дата и время начала задачи.
        /// </summary>
        public DateTime? StartDateTime { get; } = null;

        /// <summary>
        /// Дата и время окончания задачи.
        /// </summary>
        public DateTime? EndDateTime { get; } = null;

        /// <summary>
        /// Продолжительность задачи.
        /// </summary>
        public TimeSpan? Duration { get; } = null;

        /// <summary>
        /// Флаг, true - задача повторяемая, false - задача не повторяемая.
        /// </summary>
        public bool IsRepit { get; }

        /// <summary>
        /// Дата и время через которое задача должна повториться. 
        /// </summary>
        public TimeSpan? RepitTime { get; }

        /// <summary>
        /// Флаг, true - отсчитывать периодичность задачи от старта задачи, false - отсчитывать периодичность задачи от конца задачи.
        /// </summary>
        public bool IsRepitFromStart { get; }

        /// <summary>
        /// Количество повтор задачи.
        /// Если задача повторяется, то должно быть больше нуля, 
        /// кроме случаев, когда поле <see cref="EndTimeRepit"/> или <see cref="EndDateRepit"/> не равно null.
        /// </summary>
        public int CountRepit { get; } = 0;

        /// <summary>
        /// Дата и время начала повторов задачи. 
        /// Если задача повторяется, то это поле должно быть равно <see cref="StartTime"/>, 
        /// и поле <see cref="CountRepit"/> должно быть больше нуля.
        /// </summary>
        public DateTime? StartDateTimeRepit { get; }

        /// <summary>
        /// Дата и время окончания повторов задачи.
        /// </summary>
        public DateTime? EndDateTimeRepit { get; }

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
        /// Идетификатор временной позиция относительно задачи.
        /// </summary>
        public int TimePositionRegardingTaskId { get; }

        /// <summary>
        /// Идентификатор второй задачи, с которой связано правило.
        /// </summary>
        public int SecondTaskId { get; }

        /// <summary>
        /// Идетификатор отношения времени.
        /// </summary>
        public int RelationRangeId { get; }

        /// <summary>
        /// Диапазон времени между задачами.
        /// </summary>
        public TimeSpan? DateTimeRange { get; }

        /// <summary>
        /// Флаг, true - если задача выполнена, false - если задача не выполнена.
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Дата и время выполнения задачи.
        /// </summary>
        public DateTime? CompleteDateTime { get; set; }

        public TaskDatabase(
            int id, 
            int userId, 
            string name, 
            string description, 
            DateTime createdDate, 
            int priority, 
            DateTime? startDateTime, 
            DateTime? endDateTime, 
            TimeSpan? duration, 
            bool isRepit, 
            TimeSpan? repitTime, 
            bool isRepitFromStart, 
            int countRepit, 
            DateTime? startDateTimeRepit, 
            DateTime? endDateTimeRepit, 
            bool ruleOneTask, 
            DateTime? startDateTimeRuleOneTask, 
            DateTime? endDateTimeRuleOneTask, 
            bool ruleTwoTask, 
            int timePositionRegardingTaskId, 
            int secondTaskId, 
            int relationRangeId, 
            TimeSpan? dateTimeRange,
            bool isComplete,
            DateTime? completeDateTime)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Description = description;
            CreatedDate = createdDate;
            Priority = priority;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Duration = duration;
            IsRepit = isRepit;
            RepitTime = repitTime;
            IsRepitFromStart = isRepitFromStart;
            CountRepit = countRepit;
            StartDateTimeRepit = startDateTimeRepit;
            EndDateTimeRepit = endDateTimeRepit;
            RuleOneTask = ruleOneTask;
            StartDateTimeRuleOneTask = startDateTimeRuleOneTask;
            EndDateTimeRuleOneTask = endDateTimeRuleOneTask;
            RuleTwoTask = ruleTwoTask;
            TimePositionRegardingTaskId = timePositionRegardingTaskId;
            SecondTaskId = secondTaskId;
            RelationRangeId = relationRangeId;
            DateTimeRange = dateTimeRange;
            IsComplete = isComplete;
            CompleteDateTime = completeDateTime;
        }
    }
}
