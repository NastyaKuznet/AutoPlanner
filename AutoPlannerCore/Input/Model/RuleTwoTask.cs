namespace AutoPlannerCore.Input.Model
{
    /// <summary>
    /// Правило для основной задачи относительно второй задачи.
    /// </summary>
    public class RuleTwoTask : Rule
    {
        /// <summary>
        /// Временная позиция относительно задачи.
        /// </summary>
        public TimePosition TimePositionRegardingTask { get; set; }

        /// <summary>
        /// Вторая задача, с которой связано правило.
        /// </summary>
        public MyTask SecondTask { get; set; }

        /// <summary>
        /// Отношение времени.
        /// </summary>
        public RelationRangeType RelationRange { get; set; }

        /// <summary>
        /// Диапазон времени между задачами.
        /// </summary>
        public TimeSpan DateTimeRange { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (RuleTwoTask)obj;

            return Id == other.Id &&
                   string.Equals(Name, other.Name, StringComparison.Ordinal) &&
                   string.Equals(Description, other.Description, StringComparison.Ordinal) &&
                   Priority == other.Priority &&
                   TimePositionRegardingTask == other.TimePositionRegardingTask &&
                   SecondTask.Equals(other.SecondTask) &&
                   RelationRange == other.RelationRange &&
                   DateTimeRange == other.DateTimeRange;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Name);
            hash.Add(Description);
            hash.Add(Priority);
            hash.Add(TimePositionRegardingTask);
            hash.Add(SecondTask);
            hash.Add(RelationRange);
            hash.Add(DateTimeRange);
            return hash.ToHashCode();
        }
    }

    public enum TimePosition
    {
        Before = 0,
        After = 1,
    }

    public enum RelationRangeType
    {
        Greater = 0,
        Equal = 1,
        Less = 2,
    }
}
