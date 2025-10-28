using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Output.Model;
using AutoPlannerCore.Planning;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Test.PlannerTest
{
    /// <summary>
    /// Группа тестов для <see cref="Planner.FindTaskTakeUpPlace(TimeTable, MyTask, IReadOnlyCollection{MyTask})"/>.
    /// </summary>
    [TestClass]
    public class PlannerTestFindTaskTakeUpPlace
    {
        [TestMethod]
        public void FindTaskTakeUpPlaceHas()
        {
            var timeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        MyTaskId = 2,
                        StartDateTime = new DateTime(2025, 10, 1, 1, 00, 0),
                        EndDateTime = new DateTime(2025, 10, 1, 2, 05, 0),
                    }
                }
            };
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 10, 0),
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                StartDateTime = new DateTime(2025, 10, 1, 1, 00, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 05, 0),
            };
            var tasks = new List<PlanningTask>() { task1, task2 };

            var result = Planner.FindTaskTakeUpPlace(timeTable, task1, tasks);

            
            Assert.IsTrue(result.Equals(task2));
        }

        [TestMethod]
        public void FindTaskTakeUpPlaceHasNot()
        {
            var timeTable = new TimeTable()
            {
            };
            var task1 = new PlanningTask()
            {
                StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 10, 0),
            };
            var tasks = new List<PlanningTask>() { task1 };

            var result = Planner.FindTaskTakeUpPlace(timeTable, task1, tasks);

            Assert.IsTrue(result is null);
        }
    }
}
