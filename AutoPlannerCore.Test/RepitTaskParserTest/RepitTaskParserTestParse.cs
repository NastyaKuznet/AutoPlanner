using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Planning;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Test.RepitTaskParserTest
{
    /// <summary>
    /// Группа тестов для <see cref="RepitTaskParser.Parse(MyTask)"/>.
    /// </summary>
    [TestClass]
    public class RepitTaskParserTestParse
    {
        /// <summary>
        /// Задача начинается 21-09-2025 в 13:35:00 и заканчивается 21-09-2025 в 14:00:00.
        /// Она повторяется три раза через 1 час от конца задачи.
        /// </summary>
        [TestMethod]
        public void ParseBetweenHourIsRepitFromStartFalse()
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
                RepitDateTime = new TimeSpan(1, 00, 00),
                StartDateTimeRepit = new DateTime(2025, 9, 21, 13, 35, 00),
                EndDateTimeRepit = new DateTime(2025, 10, 21, 13, 35, 00),
                CountRepit = 3,
            };

            var repitTasks = RepitTaskParser.Parse(task);
            var expectedRepitTasks = new List<PlanningTask>()
            {
                new PlanningTask()
                {
                    MyTaskId = 1,
                    Name = name,
                    Description = description,
                    Priority = 1,
                    StartDateTime = new DateTime(2025, 9, 21, 13, 35, 00),
                    EndDateTime = new DateTime(2025, 9, 21, 14, 00, 00),
                    CountFrom = 1,
                },
                new PlanningTask()
                {
                    MyTaskId = 1,
                    Name = name,
                    Description = description,
                    Priority = 1,
                    StartDateTime = new DateTime(2025, 9, 21, 15, 00, 00),
                    EndDateTime = new DateTime(2025, 9, 21, 15, 25, 00),
                    CountFrom = 2,
                },
                new PlanningTask()
                {
                    MyTaskId = 1,
                    Name = name,
                    Description = description,
                    Priority = 1,
                    StartDateTime = new DateTime(2025, 9, 21, 16, 25, 00),
                    EndDateTime = new DateTime(2025, 9, 21, 16, 50, 00),
                    CountFrom = 3,
                }
            };

            Assert.IsTrue(expectedRepitTasks.SequenceEqual(repitTasks));
        }

        /// <summary>
        /// Задача начинается 21-09-2025 в 13:35:00 и заканчивается 21-09-2025 в 14:00:00.
        /// Она повторяется три раза через 1 час от начала задачи.
        /// </summary>
        [TestMethod]
        public void ParseBetweenHourIsRepitFromStartTrue()
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
                RepitDateTime = new TimeSpan(1, 00, 00),
                StartDateTimeRepit = new DateTime(2025, 9, 21, 13, 35, 00),
                EndDateTimeRepit = new DateTime(2025, 10, 21, 13, 35, 00),
                CountRepit = 3,
                IsRepitFromStart = true,
            };

            var repitTasks = RepitTaskParser.Parse(task);
            var expectedRepitTasks = new List<PlanningTask>()
            {
                new PlanningTask()
                {
                    MyTaskId = 1,
                    Name = name,
                    Description = description,
                    Priority = 1,
                    StartDateTime = new DateTime(2025, 9, 21, 13, 35, 00),
                    EndDateTime = new DateTime(2025, 9, 21, 14, 00, 00),
                    CountFrom = 1,
                },
                new PlanningTask()
                {
                    MyTaskId = 1,
                    Name = name,
                    Description = description,
                    Priority = 1,
                    StartDateTime = new DateTime(2025, 9, 21, 14, 35, 00),
                    EndDateTime = new DateTime(2025, 9, 21, 15, 00, 00),
                    CountFrom = 2,
                },
                new PlanningTask()
                {
                    MyTaskId = 1,
                    Name = name,
                    Description = description,
                    Priority = 1,
                    StartDateTime = new DateTime(2025, 9, 21, 15, 35, 00),
                    EndDateTime = new DateTime(2025, 9, 21, 16, 00, 00),
                    CountFrom = 3,
                }
            };

            Assert.IsTrue(expectedRepitTasks.SequenceEqual(repitTasks));
        }

        /// <summary>
        /// Задача начинается 21-09-2025 в 13:35:00 и заканчивается 21-09-2025 в 14:00:00.
        /// Она повторяется три раза через 1 день.
        /// </summary>
        [TestMethod]
        public void ParseBetweenDay()
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
                RepitDateTime = new TimeSpan(1, 00,  00, 00),
                StartDateTimeRepit = new DateTime(2025, 9, 21, 13, 35, 00),
                EndDateTimeRepit = new DateTime(2025, 10, 21, 13, 35, 00),
                CountRepit = 3,
                IsRepitFromStart = true,
            };

            var repitTasks = RepitTaskParser.Parse(task);
            var expectedRepitTasks = new List<PlanningTask>()
            {
                new PlanningTask()
                {
                    MyTaskId = 1,
                    Name = name,
                    Description = description,
                    Priority = 1,
                    StartDateTime = new DateTime(2025, 9, 21, 13, 35, 00),
                    EndDateTime = new DateTime(2025, 9, 21, 14, 00, 00),
                    CountFrom = 1,
                },
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
                }
            };

            Assert.IsTrue(expectedRepitTasks.SequenceEqual(repitTasks));
        }
    }
}
