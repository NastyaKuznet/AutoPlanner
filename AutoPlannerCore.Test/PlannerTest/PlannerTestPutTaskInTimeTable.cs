using AutoPlannerCore.Output.Model;
using AutoPlannerCore.Planning;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Test.PlannerTest
{
    /// <summary>
    /// Группа тестов для <see cref="Planner.PutTaskInTimeTable(TimeTable, Input.Model.MyTask)"/>.
    /// </summary>
    [TestClass]
    public class PlannerTestPutTaskInTimeTable
    {
        /// <summary>
        /// Поместить задачу в пустую таблицу.
        /// </summary>
        [TestMethod]
        public void PutTaskInTimeTable()
        {
            var timeTable = new TimeTable();
            var task = new PlanningTask()
            {
                MyTaskId = 1,
                Name = "Сходить в прачку",
                StartDateTime = new DateTime(2025, 9, 18, 8, 30, 0),
                EndDateTime = new DateTime(2025, 9, 18, 8, 40, 0),
            };

            Planner.PutTaskInTimeTable(timeTable, task);

            var expectedTimeTable = new TimeTable() 
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        MyTaskId = 1,
                        Name = "Сходить в прачку",
                        StartDateTime = new DateTime(2025, 9, 18, 8, 30, 0),
                        EndDateTime =  new DateTime(2025, 9, 18, 8, 40, 0),
                    }
                }
            };

            Assert.IsTrue(timeTable.Equals(expectedTimeTable));
        }
    }
}
