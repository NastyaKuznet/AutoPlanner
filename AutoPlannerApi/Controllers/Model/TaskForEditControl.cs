namespace AutoPlannerApi.Controllers.Model
{
    public class TaskForEditControl
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public int Priority { get; set; } = 0;

        /// <summary>
        /// Дата и время начала задачи.
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// Дата и время окончания задачи.
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Продолжительность задачи.
        /// </summary>
        public TimeSpan? Duration { get; set; } = null;

        /// <summary>
        /// Флаг, true - задача повторяемая, false - задача не повторяемая.
        /// </summary>
        public bool IsRepit { get; set; }

        /// <summary>
        /// Дата и время через которое задача должна повториться. 
        /// </summary>
        public TimeSpan? RepitTime { get; set; }

        /// <summary>
        /// Флаг, true - отсчитывать периодичность задачи от старта задачи, false - отсчитывать периодичность задачи от конца задачи.
        /// </summary>
        public bool IsRepitFromStart { get; set; }

        /// <summary>
        /// Количество повтор задачи.
        /// Если задача повторяется, то должно быть больше нуля, 
        /// кроме случаев, когда поле <see cref="EndTimeRepit"/> или <see cref="EndDateRepit"/> не равно null.
        /// </summary>
        public int CountRepit { get; set; } = 0;

        /// <summary>
        /// Дата и время начала повторов задачи. 
        /// Если задача повторяется, то это поле должно быть равно <see cref="StartTime"/>, 
        /// и поле <see cref="CountRepit"/> должно быть больше нуля.
        /// </summary>
        public DateTime? StartDateTimeRepit { get; set; }

        /// <summary>
        /// Дата и время окончания повторов задачи.
        /// </summary>
        public DateTime? EndDateTimeRepit { get; set; }

        /// <summary>
        /// Есть ли правило о допустимом диапазоне существования.
        /// </summary>
        public bool RuleOneTask { get; set; }

        /// <summary>
        /// Дата и время начала периода.
        /// </summary>
        public DateTime? StartDateTimeRuleOneTask { get; set; }

        /// <summary>
        /// Дата и время окончания периода.
        /// </summary>
        public DateTime? EndDateTimeRuleOneTask { get; set; }

        /// <summary>
        /// Есть ли правило зависимости от других задач.
        /// </summary>
        public bool RuleTwoTask { get; set; }

        /// <summary>
        /// Идетификатор временной позиция относительно задачи.
        /// </summary>
        public int TimePositionRegardingTaskId { get; set; }

        /// <summary>
        /// Идентификатор второй задачи, с которой связано правило.
        /// </summary>
        public int SecondTaskId { get; set; }

        /// <summary>
        /// Идетификатор отношения времени.
        /// </summary>
        public int RelationRangeId { get; set; }

        /// <summary>
        /// Диапазон времени между задачами.
        /// </summary>
        public TimeSpan? DateTimeRange { get; set; }


        /// <summary>
        /// Флаг, true - если задача выполнена, false - если задача не выполнена.
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Дата и время выполнения задачи.
        /// </summary>
        public DateTime? CompleteDateTime { get; set; }
    }
}
