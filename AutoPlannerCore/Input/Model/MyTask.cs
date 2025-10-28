using AutoPlannerCore.Input.Model.Contracts;

namespace AutoPlannerCore.Input.Model
{
    /// <summary>
    /// Задача.
    /// </summary>
    public class MyTask : IItem
    {
        /// <inheritdoc/>
        public int Id { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedDate { get; init; } = DateTime.Now;

        /// <inheritdoc/>
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
        public TimeSpan? RepitDateTime { get; init; }

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
        /// Правило о допустимом или не допустимом диапазоне существования. Может быть null.
        /// </summary>
        public RuleOneTask? RuleOneTask { get; init; }

        /// <summary>
        /// Правило зависимости от других задач. Может быть null.
        /// </summary>
        public RuleTwoTask? RuleTwoTask { get; init; }

        /// <summary>
        /// Флаг, true - если задача выполнена, false - если задача не выполнена.
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Дата и время выполнения задачи.
        /// </summary>
        public DateTime? CompleteDateTime { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            MyTask other = (MyTask)obj;

            return Id == other.Id &&
                   string.Equals(Name, other.Name, StringComparison.Ordinal) &&
                   string.Equals(Description, other.Description, StringComparison.Ordinal) &&
                   CreatedDate == other.CreatedDate &&
                   Priority == other.Priority &&
                   Nullable.Equals(StartDateTime, other.StartDateTime) &&
                   Nullable.Equals(EndDateTime, other.EndDateTime) &&
                   Duration == other.Duration &&
                   Nullable.Equals(RepitDateTime, other.RepitDateTime) &&
                   IsRepit == other.IsRepit &&
                   CountRepit == other.CountRepit &&
                   Nullable.Equals(StartDateTimeRepit, other.StartDateTimeRepit) &&
                   Nullable.Equals(EndDateTimeRepit, other.EndDateTimeRepit) &&
                   Nullable.Equals(RuleOneTask, other.RuleOneTask) &&
                   Nullable.Equals(RuleTwoTask, other.RuleTwoTask);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Name);
            hash.Add(Description);
            hash.Add(CreatedDate);
            hash.Add(Priority);
            hash.Add(StartDateTime);
            hash.Add(EndDateTime);
            hash.Add(Duration);
            hash.Add(RepitDateTime);
            hash.Add(IsRepit);
            hash.Add(CountRepit);
            hash.Add(IsRepitFromStart);
            hash.Add(StartDateTimeRepit);
            hash.Add(EndDateTimeRepit);
            hash.Add(RuleOneTask);
            hash.Add(RuleTwoTask);
            return hash.ToHashCode();
        }
    }
}
