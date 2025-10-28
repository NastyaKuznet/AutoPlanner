using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Output.Model;
using AutoPlannerCore.Planning;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Test.PlannerTest
{
    /// <summary>
    /// Группа тестов для <see cref="Planner.DoPlan(IReadOnlyCollection{Input.Model.MyTask}, Output.Model.TimeTable)"/>
    /// </summary>
    [TestClass]
    public class PlannerTestDoPlan
    {
        [TestMethod]
        public void DoPlanClassic()
        {
            var task1 = new MyTask()
            {
                Id = 1,
                Name = "Сходить в прачку",
                StartDateTime = new DateTime(2025, 09, 27, 8, 30, 00),
                Duration = new TimeSpan(0, 15, 0)
            };
            var task2 = new MyTask()
            {
                Id = 2,
                Name = "Сходить в прачку",
                StartDateTime = new DateTime(2025, 09, 27, 8, 30, 00),
                Duration = new TimeSpan(0, 15, 0)
            };

            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 09, 26), new DateTime(2025, 09, 30)));
            planner.DoPlan(new List<MyTask>() { task1, task2 }, table);

            var expectedTimeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        MyTaskId = task1.Id,
                        Name = task1.Name,
                        StartDateTime = new DateTime(2025, 09, 27, 8, 30, 00),
                        EndDateTime = new DateTime(2025, 09, 27, 8, 45, 00)
                    },
                }
            };
            var expectedPenaltyTask = new List<PlanningTask>()
            {
                new PlanningTask()
                {
                    MyTaskId = task2.Id,
                    Name = task1.Name,
                    StartDateTime = new DateTime(2025, 09, 27, 8, 30, 00),
                    EndDateTime = new DateTime(2025, 09, 27, 8, 45, 00)
                }
            };
            Assert.IsTrue(expectedTimeTable.Equals(table) && expectedPenaltyTask.SequenceEqual(planner.PenaltyTask));
        }
        [TestMethod]
        public void DoPlanCheckTwoSecondaryCheck()
        {
            var task1 = new MyTask()
            {
                Id = 1,
                Name = "Сходить в прачку",
                StartDateTime = new DateTime(2025, 09, 27, 8, 30, 00),
                Duration = new TimeSpan(0, 15, 0)
            };
            var task2 = new MyTask()
            {
                Id = 2,
                Name = "Собрать сушилку",
                Duration = new TimeSpan(0, 10, 0),
                RuleTwoTask = new RuleTwoTask()
                {
                    SecondTask = task1,
                    DateTimeRange = new TimeSpan(0, 15, 0),
                    TimePositionRegardingTask = TimePosition.After,
                    RelationRange = RelationRangeType.Equal
                }
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 09, 26), new DateTime(2025, 09, 30)));
            planner.DoPlan(new List<MyTask>() { task2, task1 }, table);

            var expectedTimeTable = new TimeTable() 
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        MyTaskId = task1.Id,
                        Name = task1.Name,
                        StartDateTime = new DateTime(2025, 09, 27, 8, 30, 00),
                        EndDateTime = new DateTime(2025, 09, 27, 8, 45, 00)
                    }, 
                    new TimeTableItem()
                    {
                        MyTaskId = task2.Id,
                        Name = task2.Name,
                        StartDateTime = new DateTime(2025, 09, 27, 9, 0, 0),
                        EndDateTime = new DateTime(2025, 09, 27, 9, 10, 0)
                    }
                }
            };
            Assert.IsTrue(expectedTimeTable.Equals(table));
        }

        [TestMethod]
        public void DoPlanCheckThreeSecondaryCheck()
        {
            var task1 = new MyTask()
            {
                Id = 1,
                Name = "Сходить в прачку",
                StartDateTime = new DateTime(2025, 09, 27, 8, 30, 00),
                Duration = new TimeSpan(0, 15, 0)
            };
            var task2 = new MyTask()
            {
                Id = 2,
                Name = "Собрать сушилку",
                Duration = new TimeSpan(0, 10, 0),
                RuleTwoTask = new RuleTwoTask()
                {
                    SecondTask = task1,
                    DateTimeRange = new TimeSpan(0, 15, 0),
                    TimePositionRegardingTask = TimePosition.After,
                    RelationRange = RelationRangeType.Equal
                }
            };
            var task3 = new MyTask()
            {
                Id = 3,
                Name = "Забрать вещи",
                Duration = new TimeSpan(0, 20, 0),
                RuleTwoTask = new RuleTwoTask()
                {
                    SecondTask = task2,
                    DateTimeRange = new TimeSpan(0, 15, 0),
                    TimePositionRegardingTask = TimePosition.After,
                    RelationRange = RelationRangeType.Equal
                }
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 09, 26), new DateTime(2025, 09, 30)));
            planner.DoPlan(new List<MyTask>() { task3, task2, task1 }, table);

            var expectedTimeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        MyTaskId = task1.Id,
                        Name = task1.Name,
                        StartDateTime = new DateTime(2025, 09, 27, 8, 30, 00),
                        EndDateTime = new DateTime(2025, 09, 27, 8, 45, 00)
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = task2.Id,
                        Name = task2.Name,
                        StartDateTime = new DateTime(2025, 09, 27, 9, 0, 0),
                        EndDateTime = new DateTime(2025, 09, 27, 9, 10, 0)
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = task3.Id,
                        Name = task3.Name,
                        StartDateTime = new DateTime(2025, 09, 27, 9, 25, 0),
                        EndDateTime = new DateTime(2025, 09, 27, 9, 45, 0)
                    }
                }
            };
            Assert.IsTrue(expectedTimeTable.Equals(table));
        }

        [TestMethod]
        public void DoPlanCheckThreeFromOneSecondaryCheck()
        {
            var task1 = new MyTask()
            {
                Id = 1,
                Name = "Сходить в прачку",
                StartDateTime = new DateTime(2025, 09, 27, 8, 30, 00),
                Duration = new TimeSpan(0, 15, 0)
            };
            var task2 = new MyTask()
            {
                Id = 2,
                Name = "Собрать сушилку",
                Duration = new TimeSpan(0, 10, 0),
                RuleTwoTask = new RuleTwoTask()
                {
                    SecondTask = task1,
                    DateTimeRange = new TimeSpan(0, 15, 0),
                    TimePositionRegardingTask = TimePosition.After,
                    RelationRange = RelationRangeType.Equal
                }
            };
            var task3 = new MyTask()
            {
                Id = 3,
                Name = "Забрать вещи",
                Duration = new TimeSpan(0, 10, 0),
                RuleTwoTask = new RuleTwoTask()
                {
                    SecondTask = task1,
                    DateTimeRange = new TimeSpan(0, 30, 0),
                    TimePositionRegardingTask = TimePosition.After,
                    RelationRange = RelationRangeType.Equal
                }
            };
            var table = new TimeTable();

            var planner = new Planner(new PreparingTaskForPlanner(new DateTime(2025, 09, 26), new DateTime(2025, 09, 30)));
            planner.DoPlan(new List<MyTask>() { task2, task3, task1 }, table);

            var expectedTimeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        MyTaskId = task1.Id,
                        Name = task1.Name,
                        StartDateTime = new DateTime(2025, 09, 27, 8, 30, 00),
                        EndDateTime = new DateTime(2025, 09, 27, 8, 45, 00)
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = task2.Id,
                        Name = task2.Name,
                        StartDateTime = new DateTime(2025, 09, 27, 9, 0, 0),
                        EndDateTime = new DateTime(2025, 09, 27, 9, 10, 0)
                    },
                    new TimeTableItem()
                    {
                        MyTaskId = task3.Id,
                        Name = task3.Name,
                        StartDateTime = new DateTime(2025, 09, 27, 9, 15, 0),
                        EndDateTime = new DateTime(2025, 09, 27, 9, 25, 0)
                    }
                }
            };
            Assert.IsTrue(expectedTimeTable.Equals(table));
        }
    }
}
