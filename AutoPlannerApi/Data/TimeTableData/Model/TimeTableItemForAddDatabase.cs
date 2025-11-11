namespace AutoPlannerApi.Data.TimeTableData.Model
{
    public class TimeTableItemForAddDatabase
    {
        public int MyTaskId { get; }
        /// <summary>
        /// Идентификатор пользователя, к которому принадлежит задача.
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Индетификатор, если задача повтораяемая.
        /// </summary>
        public int CountFrom { get; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc/>
        public int Priority { get; set; } = 0;

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

        public TimeTableItemForAddDatabase(
            int myTaskId,
            int userId,
            int countFrom,
            string name,
            int priority,
            DateTime startDateTime,
            DateTime endDateTime,
            bool isComplete,
            DateTime? completeDateTime)
        {
            MyTaskId = myTaskId;
            UserId = userId;
            CountFrom = countFrom;
            Name = name;
            Priority = priority;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            IsComplete = isComplete;
            CompleteDateTime = completeDateTime;
        }
    }
}
