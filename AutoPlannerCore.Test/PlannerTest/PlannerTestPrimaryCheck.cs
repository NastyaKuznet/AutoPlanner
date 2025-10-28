using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Output.Model;
using AutoPlannerCore.Planning;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Test.PlannerTest
{
    /// <summary>
    /// Группа тестов для <see cref="Planner.PrimaryCheck(List{PlanningTask}, TimeTable)"/>.
    /// </summary>
    [TestClass]
    public class PlannerTestPrimaryCheck
    {
        /// <summary>
        /// Проверка сценария №1. См. схему алгоритма.
        /// </summary>
        [TestMethod]
        public void PrimaryCheckCase1()
        {
            var task = new PlanningTask()
            {
                MyTaskId = 1,
                Name = "Сходить в прачку",
                StartDateTime = new DateTime(2025, 9, 18, 8, 30, 0),
                EndDateTime = new DateTime(2025, 9, 18, 8, 40, 0),
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 9, 17), new DateTime(2025, 9, 19)));
            planner.PrimaryCheck(new List<PlanningTask>() { task }, table, new List<PlanningTask>() { task });

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
                        CountFrom = 0,
                    }
                }
            };

            Assert.IsTrue(table.Equals(expectedTimeTable));
        }

        /// <summary>
        /// Проверка сценария №2. См. схему алгоритма.
        /// </summary>
        [TestMethod]
        public void PrimaryCheckCase2()
        {
            var task = new PlanningTask()
            {
                MyTaskId = 1,
                Name = "Сходить в прачку",
                StartDateTime = new DateTime(2025, 9, 18, 8, 30, 0),
                EndDateTime = new DateTime(2025, 9, 18, 8, 40, 0),
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 9, 17), new DateTime(2025, 9, 19)));
            planner.PrimaryCheck(new List<PlanningTask>() { task, task }, table, new List<PlanningTask>() { task, task });

            var expectedPenaltyTask = new List<PlanningTask>() { new PlanningTask()
            {
                MyTaskId = task.MyTaskId,
                Name = task.Name,
                StartDateTime = task.StartDateTime,
                EndDateTime = task.EndDateTime,
            } };

            Assert.IsTrue(planner.PenaltyTask.SequenceEqual(expectedPenaltyTask));
        }

        /// <summary>
        /// Проверка сценария №3. См. схему алгоритма.
        /// </summary>
        [TestMethod]
        public void PrimaryCheckCase3()
        {
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                Name = "Сходить в прачку",
                Priority = 1,
                StartDateTime = new DateTime(2025, 9, 18, 8, 30, 0),
                EndDateTime = new DateTime(2025, 9, 18, 8, 40, 0),
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                Name = "Сходить в прачку",
                Priority = 2,
                StartDateTime = new DateTime(2025, 9, 18, 8, 30, 0),
                EndDateTime = new DateTime(2025, 9, 18, 8, 40, 0),
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 9, 17), new DateTime(2025, 9, 19)));
            planner.PrimaryCheck(new List<PlanningTask>() { task1, task2 }, table, new List<PlanningTask>() { task1, task2 });

            var expectedPenaltyTask = new List<PlanningTask>() { new PlanningTask()
            {
                MyTaskId = task1.MyTaskId,
                Name = task1.Name,
                StartDateTime = task1.StartDateTime,
                EndDateTime = task1.EndDateTime,
                Priority = 1,
            } };

            Assert.IsTrue(planner.PenaltyTask.SequenceEqual(expectedPenaltyTask));
        }

        /// <summary>
        /// Проверка сценария №4. См. схему алгоритма.
        /// </summary>
        [TestMethod]
        public void PrimaryCheckCase4()
        {
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                Name = "Сходить в прачку 1",
                Priority = 1,
                StartDateTime = new DateTime(2025, 9, 18, 8, 30, 0),
                EndDateTime = new DateTime(2025, 9, 18, 8, 40, 0),
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                Name = "Сходить в прачку 2",
                Priority = 3,
                StartDateTime = new DateTime(2025, 9, 18, 8, 40, 0),
                EndDateTime = new DateTime(2025, 9, 18, 8, 50, 0),
            };
            var task3 = new PlanningTask()
            {
                MyTaskId = 3,
                Name = "Сходить в прачку 3",
                Priority = 2,
                StartDateTime = new DateTime(2025, 9, 18, 8, 35, 0),
                EndDateTime = new DateTime(2025, 9, 18, 8, 45, 0),
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 9, 17), new DateTime(2025, 9, 19)));
            planner.PrimaryCheck(new List<PlanningTask>() { task1, task2, task3 }, table, new List<PlanningTask>() { task1, task2, task3 });

            var expectedPenaltyTask = new List<PlanningTask>() { new PlanningTask()
            {
                MyTaskId = task3.MyTaskId,
                Name = task3.Name,
                StartDateTime = task3.StartDateTime,
                EndDateTime = task3.EndDateTime,
                Priority = 2,
            } };

            Assert.IsTrue(planner.PenaltyTask.SequenceEqual(expectedPenaltyTask));
        }

        /// <summary>
        /// Проверка сценария №5. См. схему алгоритма.
        /// </summary>
        [TestMethod]
        public void PrimaryCheckCase5()
        {
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                Name = "Сходить в прачку 1",
                Priority = 1,
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 9, 17), new DateTime(2025, 9, 19)));
            planner.PrimaryCheck(new List<PlanningTask>() { task1 }, table, new List<PlanningTask>() { task1 });

            var expectedTertiaryQueue = new List<PlanningTask>() { new PlanningTask()
            {
                MyTaskId = task1.MyTaskId,
                Name = task1.Name,
                Priority = task1.Priority,
                StartDateTimeRange = new DateTime(2025, 9, 17),
                EndDateTimeRange = new DateTime(2025, 9, 19)
            } };

            Assert.IsTrue(planner.TertiaryQueue.SequenceEqual(expectedTertiaryQueue));
        }

        /// <summary>
        /// Проверка сценария №6. См. схему алгоритма.
        /// </summary>
        [TestMethod]
        public void PrimaryCheckCase6()
        {
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                Name = "Сходить в прачку 1",
                Priority = 1,
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                Name = "Собрать сушилку",
                Priority = 1,
                RuleTwoTask = new RuleTwoTask()
                {
                    SecondTask = new MyTask()
                    {
                        Id = task1.MyTaskId,
                        Name = task1.Name,
                        Priority = 1,
                    },
                }
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 9, 17), new DateTime(2025, 9, 19)));
            planner.PrimaryCheck(new List<PlanningTask>() { task2, task1 }, table, new List<PlanningTask>() { task2, task1 });

            var expectedSecondaryQueue = new List<PlanningTask>() { new PlanningTask()
            {
                MyTaskId = task2.MyTaskId,
                Name = task2.Name,
                Priority = task2.Priority,
                RuleTwoTask = task2.RuleTwoTask,
            } };

            Assert.IsTrue(planner.SecondaryQueue.SequenceEqual(expectedSecondaryQueue));
        }

        /// <summary>
        /// Проверка сценария №7. См. схему алгоритма.
        /// </summary>
        [TestMethod]
        public void PrimaryCheckCase7()
        {
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                Name = "Сходить в прачку 1",
                Priority = 1,
                StartDateTime = new DateTime(2025, 9, 18, 8, 40, 0),
                EndDateTime = new DateTime(2025, 9, 18, 8, 50, 0),
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                Name = "Собрать сушилку",
                Priority = 1,
                RuleTwoTask = new RuleTwoTask()
                {
                    SecondTask = new MyTask()
                    {
                        Id = task1.MyTaskId,
                        Name = task1.Name,
                        Priority = 1,
                    },
                    RelationRange = RelationRangeType.Greater,
                    TimePositionRegardingTask = TimePosition.After,
                    DateTimeRange = new TimeSpan(0, 10, 0)
                }
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 9, 17), new DateTime(2025, 9, 19)));
            planner.PrimaryCheck(new List<PlanningTask>() { task1, task2 }, table, new List<PlanningTask>() { task1, task2 });

            var expectedSecondaryQueue = new List<PlanningTask>() { new PlanningTask()
            {
                MyTaskId = task2.MyTaskId,
                Name = task2.Name,
                Priority = task2.Priority,
                RuleTwoTask = task2.RuleTwoTask,
                StartDateTimeRange = new DateTime(2025, 9, 18, 9, 00, 0),
                EndDateTimeRange = new DateTime(2025, 9, 19)
            } };

            Assert.IsTrue(planner.TertiaryQueue.SequenceEqual(expectedSecondaryQueue));
        }

        /// <summary>
        /// Проверка сценария №8. См. схему алгоритма.
        /// </summary>
        [TestMethod]
        public void PrimaryCheckCase8()
        {
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                Name = "Сходить в прачку",
                Priority = 1,
                StartDateTime = new DateTime(2025, 9, 18, 8, 40, 0),
                EndDateTime = new DateTime(2025, 9, 18, 8, 50, 0),
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                Name = "Собрать сушилку",
                Priority = 1,
                Duration = new TimeSpan(0, 10, 0),
                RuleTwoTask = new RuleTwoTask()
                {
                    SecondTask = new MyTask()
                    {
                        Id = task1.MyTaskId,
                        Name = task1.Name,
                        Priority = 1,
                    },
                    RelationRange = RelationRangeType.Equal,
                    DateTimeRange = new TimeSpan(0, 15, 0),
                    TimePositionRegardingTask = TimePosition.After
                }
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 9, 17), new DateTime(2025, 9, 19)));
            planner.PrimaryCheck(new List<PlanningTask>() { task1, task2 }, table, new List<PlanningTask>() { task1, task2 });

            var expectedTimeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        MyTaskId = 1,
                        Name = "Сходить в прачку",
                        StartDateTime = new DateTime(2025, 9, 18, 8, 40, 0),
                        EndDateTime =  new DateTime(2025, 9, 18, 8, 50, 0),
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = 2,
                        Name = "Собрать сушилку",
                        StartDateTime = new DateTime(2025, 9, 18, 9, 5, 0),
                        EndDateTime =  new DateTime(2025, 9, 18, 9, 15, 0),
                    },
                }
            };

            Assert.IsTrue(table.Equals(expectedTimeTable));
        }
    }
}
