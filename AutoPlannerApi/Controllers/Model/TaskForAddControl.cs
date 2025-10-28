namespace AutoPlannerApi.Controllers.Model
{
    public class TaskForAddControl
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; init; } = DateTime.Now;

        public int Priority { get; set; } = 0;

        /// <summary>
        /// Дата и время начала задачи.
        /// </summary>
        public DateTime? StartDateTime { get; init; } = null;

        /// <summary>
        /// Дата и время окончания задачи.
        /// </summary>
        public DateTime? EndDateTime { get; init; } = null;

        /// <summary>
        /// Продолжительность задачи.
        /// </summary>
        public TimeSpan? Duration { get; init; } = null;

        /// <summary>
        /// Флаг, true - задача повторяемая, false - задача не повторяемая.
        /// </summary>
        public bool IsRepit { get; set; }

        /// <summary>
        /// Дата и время через которое задача должна повториться. 
        /// </summary>
        public TimeSpan? RepitTime { get; init; }

        /// <summary>
        /// Флаг, true - отсчитывать периодичность задачи от старта задачи, false - отсчитывать периодичность задачи от конца задачи.
        /// </summary>
        public bool IsRepitFromStart { get; set; }

        /// <summary>
        /// Количество повтор задачи.
        /// Если задача повторяется, то должно быть больше нуля, 
        /// кроме случаев, когда поле <see cref="EndTimeRepit"/> или <see cref="EndDateRepit"/> не равно null.
        /// </summary>
        public int CountRepit { get; init; } = 0;

        /// <summary>
        /// Дата и время начала повторов задачи. 
        /// Если задача повторяется, то это поле должно быть равно <see cref="StartTime"/>, 
        /// и поле <see cref="CountRepit"/> должно быть больше нуля.
        /// </summary>
        public DateTime? StartDateTimeRepit { get; init; }

        /// <summary>
        /// Дата и время окончания повторов задачи.
        /// </summary>
        public DateTime? EndDateTimeRepit { get; init; }

        /// <summary>
        /// Есть ли правило о допустимом диапазоне существования.
        /// </summary>
        public bool RuleOneTask { get; init; }

        /// <summary>
        /// Дата и время начала периода.
        /// </summary>
        public DateTime StartDateTimeRuleOneTask { get; init; }

        /// <summary>
        /// Дата и время окончания периода.
        /// </summary>
        public DateTime EndDateTimeRuleOneTask { get; init; }

        /// <summary>
        /// Есть ли правило зависимости от других задач.
        /// </summary>
        public bool RuleTwoTask { get; init; }

        /// <summary>
        /// Идентификатор временной позиция относительно задачи.
        /// </summary>
        public int TimePositionRegardingTaskId { get; set; }

        /// <summary>
        /// Идентификатор второй задачи, с которой связано правило.
        /// </summary>
        public int SecondTaskId { get; set; }

        /// <summary>
        /// Идентификатор отношения времени.
        /// </summary>
        public int RelationRangeId { get; set; }

        /// <summary>
        /// Диапазон времени между задачами.
        /// </summary>
        public TimeSpan? DateTimeRange { get; set; }
    }
}
