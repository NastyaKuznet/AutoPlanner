using AutoPlannerCore.Input.Model.Contracts;

namespace AutoPlannerCore.Input.Model
{
    /// <summary>
    /// Правило.
    /// </summary>
    public abstract class Rule : IItem
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
    }
}
