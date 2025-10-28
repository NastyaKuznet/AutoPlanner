using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Planning
{
    /// <summary>
    /// Преобразователь задачи из <see cref="MyTask"/> в несколько <see cref="PlanningTask"/>, если <see cref="MyTask.IsRepit"/> = true.
    /// </summary>
    public class RepitTaskParser
    {
        /// <summary>
        /// Преобразует задачу <see cref="MyTask"/> в несколько <see cref="PlanningTask"/>, если <see cref="MyTask.IsRepit"/> = true.
        /// </summary>
        /// <param name="task">Периодичная задача.</param>
        /// <returns>Список задач с одинаковой периодичностью.</returns>
        public static List<PlanningTask> Parse(MyTask task)
        {
            var planningTasks = new List<PlanningTask>();
            var count = 0;
            var startDateTime = DateTime.MinValue;
            var endDateTime = DateTime.MinValue;
            while (count < task.CountRepit && endDateTime < task.EndDateTimeRepit)
            {
                if (task.IsRepitFromStart)
                {
                    startDateTime = (DateTime)(task.StartDateTimeRepit + task.RepitDateTime * count);
                }
                else
                {
                    startDateTime = (DateTime)(task.StartDateTimeRepit + (task.RepitDateTime + (task.EndDateTime - task.StartDateTime)) * count);
                }
                endDateTime = (DateTime)(startDateTime + (task.EndDateTime - task.StartDateTime));
                var repitTask = new PlanningTask()
                {
                    MyTaskId = task.Id,
                    Name = task.Name,
                    Description = task.Description,
                    Priority = task.Priority,
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime,
                    CountFrom = ++count,
                    Duration = task.Duration,
                    IsComplete = task.IsComplete,
                    CompleteDateTime = task.CompleteDateTime,
                };
                planningTasks.Add(repitTask);
            }
            return planningTasks;
        }
    }
}
