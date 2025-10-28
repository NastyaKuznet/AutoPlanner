using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Planning;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Test.PreparingTaskForPlannerTest
{
    /// <summary>
    /// Группа тестов для <see cref="PreparingTaskForPlanner.Prepare(IReadOnlyCollection{MyTask})"/>.
    /// </summary>
    [TestClass]
    public class PreparingTaskForPlannerTestPrepare
    {
        [TestMethod]
        public void PrepareCheckRepitTask()
        {
            var name = "test";
            var description = "description";
            var task = new MyTask()
            {
                Id = 1,
                Name = name,
                Description = description,
                Priority = 1,
                StartDateTime = new DateTime(2025, 9, 21, 13, 35, 00),
                EndDateTime = new DateTime(2025, 9, 21, 14, 00, 00),
                IsRepit = true,
                RepitDateTime = new TimeSpan(1, 00, 00, 00),
                StartDateTimeRepit = new DateTime(2025, 9, 21, 13, 35, 00),
                EndDateTimeRepit = new DateTime(2025, 10, 21, 13, 35, 00),
                CountRepit = 3,
                IsRepitFromStart = true,
            };

            var preparingTaskForPlanner = new PreparingTaskForPlanner(new DateTime(2025, 9, 22), new DateTime(2025, 9, 25));
            var planningTasks = preparingTaskForPlanner.Prepare(new List<MyTask>() 
            {
                task,
            });
            var expectedPlanningTasks = new List<PlanningTask>()
            {
                new PlanningTask()
                {
                    MyTaskId = 1,
                    Name = name,
                    Description = description,
                    Priority = 1,
                    StartDateTime = new DateTime(2025, 9, 22, 13, 35, 00),
                    EndDateTime = new DateTime(2025, 9, 22, 14, 00, 00),
                    CountFrom = 2,
                },
                new PlanningTask()
                {
                    MyTaskId = 1,
                    Name = name,
                    Description = description,
                    Priority = 1,
                    StartDateTime = new DateTime(2025, 9, 23, 13, 35, 00),
                    EndDateTime = new DateTime(2025, 9, 23, 14, 00, 00),
                    CountFrom = 3,
                },
            };

            Assert.IsTrue(expectedPlanningTasks.SequenceEqual(planningTasks));
        }

    }
}
