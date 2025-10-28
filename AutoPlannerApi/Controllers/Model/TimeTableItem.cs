namespace AutoPlannerApi.Controllers.Model
{
    public class TimeTableItem
    {
        /// <summary>
        /// Уникальный индетификатор.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Индетификатор, если задача повтораяемая.
        /// </summary>
        public int CountFrom { get; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Дата и время начала задачи в расписании.
        /// </summary>
        public DateTime StartDateTime { get; }

        /// <summary>
        /// Дата и время окончания задачи в расписании.
        /// </summary>
        public DateTime EndDateTime { get; }

        /// <summary>
        /// Флаг, true - если задача выполнена, false - если задача не выполнена.
        /// </summary>
        public bool IsComplete { get; }

        /// <summary>
        /// Дата и время выполнения задачи.
        /// </summary>
        public DateTime? CompleteDateTime { get; }

        public TimeTableItem(
            int id, 
            int countFrom, 
            string name, 
            DateTime startDateTime, 
            DateTime endDateTime, 
            bool isComplete, 
            DateTime? completeDateTime)
        {
            Id = id;
            CountFrom = countFrom;
            Name = name;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            IsComplete = isComplete;
            CompleteDateTime = completeDateTime;
        }
    }
}
