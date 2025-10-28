using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Output.Model;
using AutoPlannerCore.Planning;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Test.PlannerTest
{
    /// <summary>
    /// Группа тестов для <see cref="Planner.CheckFree(TimeTable, MyTask)"/>.
    /// </summary>
    [TestClass]
    public class PlanerTestCheckFree
    {
        /// <summary>
        /// Таблица пустая.
        /// Новая задача начинается в 1:10 и заканчивается в 2:10.
        /// Новую задачу можно поместить.
        /// </summary>
        [TestMethod]
        public void CheckFreeInEmptyTable()
        {
            var timeTable = new TimeTable();
            var task1 = new PlanningTask()
            {
                StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 10, 0),
            };

            var result = Planner.CheckFree(timeTable, task1);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// В таблице есть задача которая начинается в 1:20 и заканчивается в 2:20.
        /// Новая задача начинается в 1:10 и заканчивается в 2:10.
        /// Новую задачу нельзя поместить в таблицу.
        /// </summary>
        [TestMethod]
        public void CheckFreeInNotEmptyTableCase1()
        {
            var timeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                        EndDateTime = new DateTime(2025, 10, 1, 2, 20, 0),
                    },
                }
            };
            var task1 = new PlanningTask()
            {
                StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 20, 0),
            };

            var result = Planner.CheckFree(timeTable, task1);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// В таблице есть задача которая начинается в 1:20 и заканчивается в 2:05.
        /// Новая задача начинается в 1:10 и заканчивается в 2:10.
        /// Новую задачу нельзя поместить в таблицу.
        /// </summary>
        [TestMethod]
        public void CheckFreeInNotEmptyTableCase2()
        {
            var timeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                        EndDateTime = new DateTime(2025, 10, 1, 2, 05, 0),
                    }
                }
            };
            var task1 = new PlanningTask()
            {
                StartDateTime = new DateTime(2025,10, 1, 1, 10, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 10, 0),
            };

            var result = Planner.CheckFree(timeTable, task1);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// В таблице есть задача которая начинается в 1:00 и заканчивается в 2:05.
        /// Новая задача начинается в 1:10 и заканчивается в 2:10.
        /// Новую задачу нельзя поместить в таблицу.
        /// </summary>
        [TestMethod]
        public void CheckFreeInNotEmptyTableCase3()
        {
            var timeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        StartDateTime = new DateTime(2025, 10, 1, 1, 00, 0),
                        EndDateTime = new DateTime(2025, 10, 1, 2, 05, 0),
                    }
                }
            };
            var task1 = new PlanningTask()
            {
                StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 10, 0),
            };

            var result = Planner.CheckFree(timeTable, task1);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// В таблице есть задача которая начинается в 1:00 и заканчивается в 3:00.
        /// Новая задача начинается в 1:10 и заканчивается в 2:10.
        /// Новую задачу нельзя поместить в таблицу.
        /// </summary>
        [TestMethod]
        public void CheckFreeInNotEmptyTableCase4()
        {
            var timeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        StartDateTime = new DateTime(2025, 10, 1, 1, 00, 0),
                        EndDateTime = new DateTime(2025,10,1, 3, 00, 0),
                    }
                }
            };
            var task1 = new PlanningTask()
            {
                StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 10, 0),
            };

            var result = Planner.CheckFree(timeTable, task1);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// В таблице есть задача которая начинается в 00:00 и заканчивается в 1:00.
        /// Новая задача начинается в 1:10 и заканчивается в 2:10.
        /// Новую задачу можно поместить в таблицу.
        /// </summary>
        [TestMethod]
        public void CheckFreeInNotEmptyTableCase5()
        {
            var timeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        StartDateTime = new DateTime(2025, 10, 1, 0, 00, 0),
                        EndDateTime = new DateTime(2025, 10, 1, 1, 00, 0),
                    }
                }
            };
            var task1 = new PlanningTask()
            {
                StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 10, 0),
            };

            var result = Planner.CheckFree(timeTable, task1);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// В таблице есть задача которая начинается в 03:00 и заканчивается в 04:00.
        /// Новая задача начинается в 1:10 и заканчивается в 2:10.
        /// Новую задачу можно поместить в таблицу.
        /// </summary>
        [TestMethod]
        public void CheckFreeInNotEmptyTableCase6()
        {
            var timeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        StartDateTime = new DateTime(2025, 10, 1, 3, 00, 0),
                        EndDateTime = new DateTime(2025, 10, 1, 4, 00, 0),
                    }
                }
            };
            var task1 = new PlanningTask()
            {
                StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 10, 0),
            };

            var result = Planner.CheckFree(timeTable, task1);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// В таблице есть задача которая начинается в 01:10 и заканчивается в 2:10 2.10.2025.
        /// Новая задача начинается в 1:10 и заканчивается в 2:10 1.10.2025.
        /// Новую задачу можно поместить в таблицу.
        /// </summary>
        [TestMethod]
        public void CheckFreeInNotEmptyTableCase7()
        {
            var timeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        StartDateTime = new DateTime(2025, 10, 2, 1, 10, 0),
                        EndDateTime = new DateTime(2025, 10, 2, 2, 10, 0),
                    }
                }
            };
            var task1 = new PlanningTask()
            {
                StartDateTime = new DateTime(2025, 10, 1, 1, 10, 0),
                EndDateTime = new DateTime(2025, 10, 1, 2, 10, 0),
            };

            var result = Planner.CheckFree(timeTable, task1);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// В таблице есть задача которая начинается в 23:10 1.10.2025 и заканчивается в 2:10 2.10.2025.
        /// Новая задача начинается в 23:05 1.10.2025 и заканчивается в 2:15 2.10.2025.
        /// Новую задачу можно поместить в таблицу.
        /// </summary>
        [TestMethod]
        public void CheckFreeInNotEmptyTableCase8()
        {
            var timeTable = new TimeTable()
            {
                TimeTableItems = new List<TimeTableItem>()
                {
                    new TimeTableItem()
                    {
                        StartDateTime = new DateTime(2025, 10, 1, 23, 10, 0),
                        EndDateTime = new DateTime(2025, 10, 2, 2, 10, 0),
                    }
                }
            };
            var task1 = new PlanningTask()
            {
                StartDateTime = new DateTime(2025, 10, 1, 23, 5, 0),
                EndDateTime = new DateTime(2025, 10, 2, 2, 15, 0),
            };

            var result = Planner.CheckFree(timeTable, task1);

            Assert.IsFalse(result);
        }
    }
}
