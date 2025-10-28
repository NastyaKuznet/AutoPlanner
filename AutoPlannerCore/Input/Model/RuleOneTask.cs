using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Input.Model
{
    /// <summary>
    /// Правило на допустимый или недопустимый период существования задачи.
    /// </summary>
    public class RuleOneTask : Rule
    {
        /// <summary>
        /// Флаг, где true - существование задачи в указанном периоде допустимо, false - не допустимо.
        /// </summary>
        public bool Is { get; init; }

        /// <summary>
        /// Дата и время начала периода.
        /// </summary>
        public DateTime StartDateTime { get; init; }

        /// <summary>
        /// Дата и время окончания периода.
        /// </summary>
        public DateTime EndDateTime { get; init; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (RuleOneTask)obj;

            return Id == other.Id &&
                   string.Equals(Name, other.Name, StringComparison.Ordinal) &&
                   string.Equals(Description, other.Description, StringComparison.Ordinal) &&
                   Priority == other.Priority &&
                   StartDateTime.Equals(other.StartDateTime) &&
                   EndDateTime.Equals(other.EndDateTime) &&
                   Is == other.Is;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Name);
            hash.Add(Description);
            hash.Add(Priority);
            hash.Add(StartDateTime);
            hash.Add(EndDateTime);
            hash.Add(Is);
            return hash.ToHashCode();
        }
    }
}
