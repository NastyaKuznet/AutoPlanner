namespace AutoPlannerCore.Output.Model
{
    /// <summary>
    /// Расписания.
    /// </summary>
    public class TimeTable
    {
        /// <summary>
        /// Уникальный индетификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Элементы расписания.
        /// </summary>
        public List<TimeTableItem> TimeTableItems { get; set; } = new List<TimeTableItem>();

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            TimeTable other = (TimeTable)obj;

            if (Id != other.Id) return false;

            if (TimeTableItems == null && other.TimeTableItems == null) return true;
            if (TimeTableItems == null || other.TimeTableItems == null) return false;

            if (TimeTableItems.Count != other.TimeTableItems.Count) return false;

            if (!TimeTableItems.SequenceEqual(other.TimeTableItems)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            if (TimeTableItems != null)
            {
                foreach (var day in TimeTableItems)
                {
                    hash.Add(day);
                }
            }

            return hash.ToHashCode();
        }
    }
}
