using AutoPlannerCore.Input.Model;

namespace AutoPlannerCore.Planning.Model
{
    /// <summary>
    /// Задача, которую необходимо поместить в расписание.
    /// </summary>
    public class PlanningTask
    {
        /// <summary>
        /// Уникальный индетификатор задачи <see cref="MyTask"/>.
        /// </summary>
        public int MyTaskId { get; set; }

        /// <summary>
        /// Названия элемента.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описение задачи.
        /// </summary>
        public string Description { get; set; }

        /// <inheritdoc/>
        public int Priority { get; set; } = 0;

        /// <summary>
        /// Дата и время начала задачи.
        /// </summary>
        public DateTime? StartDateTime { get; set; } = null;

        /// <summary>
        /// Дата и время окончания задачи.
        /// </summary>
        public DateTime? EndDateTime { get; set; } = null;

        /// <summary>
        /// Длительность задачи, может быть null.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Номер задачи из обшего множества повторяемой задачи.
        /// </summary>
        public int CountFrom { get; init; }

        /// <summary>
        /// Флаг, true - если задача выполнена, false - если задача не выполнена.
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Дата и время выполнения задачи.
        /// </summary>
        public DateTime? CompleteDateTime { get; set; }

        /// <summary>
        /// Начала диапазона даты и времени для установки задачи.
        /// </summary>
        public DateTime? StartDateTimeRange { get; set; }

        /// <summary>
        /// Крнец дипазона даты и времени для установки задачи.
        /// </summary>
        public DateTime? EndDateTimeRange { get; set; }

        /// <summary>
        /// Правило о допустимом или не допустимом диапазоне существования. Может быть null.
        /// </summary>
        public RuleOneTask? RuleOneTask { get; init; }

        /// <summary>
        /// Правило зависимости от других задач. Может быть null.
        /// </summary>
        public RuleTwoTask? RuleTwoTask { get; init; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (PlanningTask)obj;

            return MyTaskId == other.MyTaskId &&
                   string.Equals(Name, other.Name, StringComparison.Ordinal) &&
                   string.Equals(Description, other.Description, StringComparison.Ordinal) &&
                   Priority == other.Priority &&
                   Nullable.Equals(StartDateTime, other.StartDateTime) &&
                   Nullable.Equals(EndDateTime, other.EndDateTime) &&
                   CountFrom == other.CountFrom &&
                   IsComplete == other.IsComplete &&
                   Nullable.Equals(CompleteDateTime, other.CompleteDateTime) &&
                   Nullable.Equals(StartDateTimeRange, other.StartDateTimeRange) &&
                   Nullable.Equals(EndDateTimeRange, other.EndDateTimeRange) &&
                   Nullable.Equals(RuleOneTask, other.RuleOneTask) &&
                   Nullable.Equals(RuleTwoTask, other.RuleTwoTask);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Name);
            hash.Add(Description);
            hash.Add(Priority);
            hash.Add(StartDateTime);
            hash.Add(EndDateTime);
            hash.Add(CountFrom);
            hash.Add(IsComplete);
            hash.Add(CompleteDateTime);
            hash.Add(StartDateTimeRange);
            hash.Add(EndDateTimeRange);
            hash.Add(RuleOneTask);
            hash.Add(RuleTwoTask);
            return hash.ToHashCode();
        }
    }
}
