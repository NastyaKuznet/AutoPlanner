using AutoPlannerCore.Output.Model;
using AutoPlannerCore.Planning;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Test.PlannerTest
{
    /// <summary>
    /// Группа тестов для <see cref="Planner.TertiaryCheck(Output.Model.TimeTable, IReadOnlyCollection{Planning.Model.PlanningTask})"/>.
    /// </summary>
    [TestClass]
    public class PlannerTestTertiaryCheck
    {
        [TestMethod]
        public void TertiaryCheckCase1()
        {
            var table = new TimeTable();
            var task1 = new PlanningTask()
            {
                StartDateTimeRange = new DateTime(2025, 10, 17, 10, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
            };

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 10, 10), new DateTime(2025, 12, 10)));
            planner.TertiaryQueue.Add(task1);
            planner.TertiaryCheck(table, new List<PlanningTask>{ task1 });

            var expectedTimeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        StartDateTime = new DateTime(2025, 10, 17, 10, 00, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 11, 00, 00),
                    }
                }
            };

            Assert.IsTrue(table.Equals(expectedTimeTable));
        }

        [TestMethod]
        public void TertiaryCheckCase2_1()
        {
            var table = new TimeTable();
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                StartDateTimeRange = new DateTime(2025, 10, 17, 10, 30, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 2,
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                StartDateTimeRange = new DateTime(2025, 10, 17, 10, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 1,
            };

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 10, 10), new DateTime(2025, 12, 10)));
            planner.TertiaryQueue.Add(task2);
            planner.TertiaryQueue.Add(task1);
            planner.TertiaryCheck(table, new List<PlanningTask> { task2, task1 });

            var expectedTimeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        MyTaskId = 1,
                        StartDateTime = new DateTime(2025, 10, 17, 11, 00, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 12, 00, 00),
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = 2,
                        StartDateTime = new DateTime(2025, 10, 17, 10, 00, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 11, 00, 00),
                    }
                }
            };

            Assert.IsTrue(table.Equals(expectedTimeTable));
        }

        [TestMethod]
        public void TertiaryCheckCase2_2()
        {
            var table = new TimeTable();
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                StartDateTimeRange = new DateTime(2025, 10, 17, 09, 50, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 2,
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                StartDateTimeRange = new DateTime(2025, 10, 17, 10, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 1,
            };

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 10, 10), new DateTime(2025, 12, 10)));
            planner.TertiaryQueue.Add(task2);
            planner.TertiaryQueue.Add(task1);
            planner.TertiaryCheck(table, new List<PlanningTask> { task2, task1 });

            var expectedTimeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        MyTaskId = 1,
                        StartDateTime = new DateTime(2025, 10, 17, 11, 00, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 12, 00, 00),
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = 2,
                        StartDateTime = new DateTime(2025, 10, 17, 10, 00, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 11, 00, 00),
                    }
                }
            };

            Assert.IsTrue(table.Equals(expectedTimeTable));
        }

        [TestMethod]
        public void TertiaryCheckCase2_3()
        {
            var table = new TimeTable();
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                StartDateTimeRange = new DateTime(2025, 10, 17, 10, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 2,
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                StartDateTimeRange = new DateTime(2025, 10, 17, 11, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 1,
            };
            var task3 = new PlanningTask()
            {
                MyTaskId = 3,
                StartDateTimeRange = new DateTime(2025, 10, 17, 9, 30, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 1,
            };

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 10, 10), new DateTime(2025, 12, 10)));
            planner.TertiaryQueue.Add(task2);
            planner.TertiaryQueue.Add(task1);
            planner.TertiaryQueue.Add(task3);
            planner.TertiaryCheck(table, new List<PlanningTask> { task2, task1, task3 });

            var expectedTimeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        MyTaskId = 2,
                        StartDateTime = new DateTime(2025, 10, 17, 11, 30, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 12, 30, 00),
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = 1,
                        StartDateTime = new DateTime(2025, 10, 17, 10, 30, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 11, 30, 00),
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = 3,
                        StartDateTime = new DateTime(2025, 10, 17, 9, 30, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 10, 30, 00),
                    },
                }
            };

            Assert.IsTrue(table.Equals(expectedTimeTable));
        }

        [TestMethod]
        public void TertiaryCheckCase2_4()
        {
            var table = new TimeTable();
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                StartDateTimeRange = new DateTime(2025, 10, 17, 10, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 2,
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                StartDateTimeRange = new DateTime(2025, 10, 17, 11, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 1,
            };
            var task3 = new PlanningTask()
            {
                MyTaskId = 3,
                StartDateTimeRange = new DateTime(2025, 10, 17, 9, 30, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 1,
            };
            var task4 = new PlanningTask()
            {
                MyTaskId = 4,
                StartDateTimeRange = new DateTime(2025, 10, 17, 12, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 1,
            };

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 10, 10), new DateTime(2025, 12, 10)));
            planner.TertiaryQueue.Add(task2);
            planner.TertiaryQueue.Add(task1);
            planner.TertiaryQueue.Add(task3);
            planner.TertiaryQueue.Add(task4);
            planner.TertiaryCheck(table, new List<PlanningTask> { task2, task1, task3, task4 });

            var expectedTimeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    
                    new TimeTableItem()
                    {
                        MyTaskId = 1,
                        StartDateTime = new DateTime(2025, 10, 17, 10, 30, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 11, 30, 00),
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = 3,
                        StartDateTime = new DateTime(2025, 10, 17, 9, 30, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 10, 30, 00),
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = 2,
                        StartDateTime = new DateTime(2025, 10, 17, 13, 00, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 14, 00, 00),
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = 4,
                        StartDateTime = new DateTime(2025, 10, 17, 12, 00, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 13, 00, 00),
                    },
                }
            };

            Assert.IsTrue(table.Equals(expectedTimeTable));
        }

        [TestMethod]
        public void TertiaryCheckCase2_5()
        {
            var table = new TimeTable();
            var task1 = new PlanningTask()
            {
                MyTaskId = 1,
                StartDateTimeRange = new DateTime(2025, 10, 17, 10, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 2,
            };
            var task2 = new PlanningTask()
            {
                MyTaskId = 2,
                StartDateTimeRange = new DateTime(2025, 10, 17, 11, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 1,
            };
            var task3 = new PlanningTask()
            {
                MyTaskId = 3,
                StartDateTimeRange = new DateTime(2025, 10, 17, 9, 30, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 1,
            };
            var task4 = new PlanningTask()
            {
                MyTaskId = 4,
                StartDateTimeRange = new DateTime(2025, 10, 17, 12, 00, 00),
                EndDateTimeRange = new DateTime(2025, 10, 19, 10, 00, 00),
                Duration = new TimeSpan(1, 00, 00),
                Priority = 1,
            };

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 10, 10), new DateTime(2025, 12, 10)));
            planner.TertiaryQueue.Add(task2);
            planner.TertiaryQueue.Add(task1);
            planner.TertiaryQueue.Add(task4);
            planner.TertiaryQueue.Add(task3);
            planner.TertiaryCheck(table, new List<PlanningTask> { task2, task1, task3, task4 });

            var expectedTimeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                     new TimeTableItem()
                    {
                        MyTaskId = 4,
                        StartDateTime = new DateTime(2025, 10, 17, 12, 30, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 13, 30, 00),
                    },
                     new TimeTableItem()
                    {
                        MyTaskId = 2,
                        StartDateTime = new DateTime(2025, 10, 17, 11, 30, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 12, 30, 00),
                    },
                     new TimeTableItem()
                    {
                        MyTaskId = 3,
                        StartDateTime = new DateTime(2025, 10, 17, 9, 30, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 10, 30, 00),
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = 1,
                        StartDateTime = new DateTime(2025, 10, 17, 10, 30, 00),
                        EndDateTime =  new DateTime(2025, 10, 17, 11, 30, 00),
                    },
                }
            };

            Assert.IsTrue(table.Equals(expectedTimeTable));
        }
    }
}
