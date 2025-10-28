namespace AutoPlannerCore.Input.Model.Contracts
{
    /// <summary>
    /// Составной элемент планера входных данных.
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Уникальный индетификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Названия элемента.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание элемента.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата создания элемента.
        /// </summary>
        public DateTime CreatedDate { get; init; }

        /// <summary>
        /// Приоритет от 0 до 9.
        /// </summary>
        public int Priority { get; set; }
    }
}
