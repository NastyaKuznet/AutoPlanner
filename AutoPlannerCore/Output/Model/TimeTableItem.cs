using AutoPlannerCore.Input.Model;

namespace AutoPlannerCore.Output.Model
{
    /// <summary>
    /// Задача как элемент расписания.
    /// </summary>
    public class TimeTableItem
    { 
        /// <summary>
        /// Уникальный индетификатор. Должен быть равен <see cref="MyTask.Id"/>.
        /// </summary>
        public int MyTaskId { get; set; }

        /// <summary>
        /// Индетификатор, если задача повтораяемая.
        /// </summary>
        public int CountFrom { get; set; }

        /// <summary>
        /// Название. Должно быть равно <see cref="MyTask.Name"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата и время начала задачи в расписании.
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Дата и время окончания задачи в расписании.
        /// </summary>
        public DateTime EndDateTime { get; set; }

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

            TimeTableItem other = (TimeTableItem)obj;

            return MyTaskId == other.MyTaskId &&
                   string.Equals(Name, other.Name, StringComparison.Ordinal) &&
                   StartDateTime == other.StartDateTime &&
                   EndDateTime == other.EndDateTime &&
                   IsComplete == other.IsComplete &&
                   Nullable.Equals(CompleteDateTime, other.CompleteDateTime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MyTaskId, Name, StartDateTime, EndDateTime, IsComplete, CompleteDateTime);
        }
    }
}
