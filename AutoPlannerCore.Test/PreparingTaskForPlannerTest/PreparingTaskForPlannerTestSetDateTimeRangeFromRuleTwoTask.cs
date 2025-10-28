using AutoPlannerCore.Input.Model;
using AutoPlannerCore.Output.Model;
using AutoPlannerCore.Planning;
using AutoPlannerCore.Planning.Model;

namespace AutoPlannerCore.Test.PreparingTaskForPlannerTest
{
    /// <summary>
    /// Группа тестов для <see cref="PreparingTaskForPlanner.SetDateTimeRangeFromRuleTwoTask(Planning.Model.PlanningTask, Output.Model.TimeTableItem)"/>.
    /// </summary>
    [TestClass]
    public class PreparingTaskForPlannerTestSetDateTimeRangeFromRuleTwoTask
    {
        [TestMethod]
        public void SetDateTimeRangeFromRuleTwoTaskIsGoodGreaterBefore()
        {
            var secondTaskTimeTableItem = new TimeTableItem()
            {
                StartDateTime = new DateTime(2025, 09, 26, 18, 20, 00)
            };
            var task = new PlanningTask()
            {
                RuleTwoTask = new RuleTwoTask()
                {
                    RelationRange = RelationRangeType.Greater,
                    TimePositionRegardingTask = TimePosition.Before,
                    DateTimeRange = new TimeSpan(1, 00, 00)
                }
            };

            var startTimeTable = new DateTime(2025, 09, 25);
            var endTimeTable = new DateTime(2025, 09, 27);
            var preparingTaskForPlanner = new PreparingTaskForPlanner(startTimeTable, endTimeTable);
            preparingTaskForPlanner.SetDateTimeRangeFromRuleTwoTask(task, secondTaskTimeTableItem);

            var expectedTask = new PlanningTask()
            {
                RuleTwoTask = task.RuleTwoTask,
                StartDateTimeRange = startTimeTable,
                EndDateTimeRange = new DateTime(2025, 09, 26, 17, 20, 00)
            };
            Assert.IsTrue(expectedTask.Equals(task));
        }

        [TestMethod]
        public void SetDateTimeRangeFromRuleTwoTaskIsGoodLessBefore()
        {
            var secondTaskTimeTableItem = new TimeTableItem()
            {
                StartDateTime = new DateTime(2025, 09, 26, 18, 20, 00)
            };
            var task = new PlanningTask()
            {
                RuleTwoTask = new RuleTwoTask()
                {
                    RelationRange = RelationRangeType.Less,
                    TimePositionRegardingTask = TimePosition.Before,
                    DateTimeRange = new TimeSpan(1, 00, 00)
                }
            };

            var startTimeTable = new DateTime(2025, 09, 25);
            var endTimeTable = new DateTime(2025, 09, 27);
            var preparingTaskForPlanner = new PreparingTaskForPlanner(startTimeTable, endTimeTable);
            preparingTaskForPlanner.SetDateTimeRangeFromRuleTwoTask(task, secondTaskTimeTableItem);

            var expectedTask = new PlanningTask()
            {
                RuleTwoTask = task.RuleTwoTask,
                StartDateTimeRange = new DateTime(2025, 09, 26, 17, 20, 00),
                EndDateTimeRange = secondTaskTimeTableItem.StartDateTime,
            };
            Assert.IsTrue(expectedTask.Equals(task));
        }


        [TestMethod]
        public void SetDateTimeRangeFromRuleTwoTaskIsGoodGreaterAfter()
        {
            var secondTaskTimeTableItem = new TimeTableItem()
            {
                EndDateTime = new DateTime(2025, 09, 26, 18, 20, 00)
            };
            var task = new PlanningTask()
            {
                RuleTwoTask = new RuleTwoTask()
                {
                    RelationRange = RelationRangeType.Greater,
                    TimePositionRegardingTask = TimePosition.After,
                    DateTimeRange = new TimeSpan(1, 00, 00)
                }
            };

            var startTimeTable = new DateTime(2025, 09, 25);
            var endTimeTable = new DateTime(2025, 09, 27);
            var preparingTaskForPlanner = new PreparingTaskForPlanner(startTimeTable, endTimeTable);
            preparingTaskForPlanner.SetDateTimeRangeFromRuleTwoTask(task, secondTaskTimeTableItem);

            var expectedTask = new PlanningTask()
            {
                RuleTwoTask = task.RuleTwoTask,
                StartDateTimeRange = secondTaskTimeTableItem.EndDateTime + new TimeSpan(1, 00, 00),
                EndDateTimeRange = endTimeTable,
            };
            Assert.IsTrue(expectedTask.Equals(task));
        }

        [TestMethod]
        public void SetDateTimeRangeFromRuleTwoTaskIsGoodLessAfter()
        {
            var secondTaskTimeTableItem = new TimeTableItem()
            {
                EndDateTime = new DateTime(2025, 09, 26, 18, 20, 00)
            };
            var task = new PlanningTask()
            {
                RuleTwoTask = new RuleTwoTask()
                {
                    RelationRange = RelationRangeType.Less,
                    TimePositionRegardingTask = TimePosition.After,
                    DateTimeRange = new TimeSpan(1, 00, 00)
                }
            };

            var startTimeTable = new DateTime(2025, 09, 25);
            var endTimeTable = new DateTime(2025, 09, 27);
            var preparingTaskForPlanner = new PreparingTaskForPlanner(startTimeTable, endTimeTable);
            preparingTaskForPlanner.SetDateTimeRangeFromRuleTwoTask(task, secondTaskTimeTableItem);

            var expectedTask = new PlanningTask()
            {
                RuleTwoTask = task.RuleTwoTask,
                StartDateTimeRange = secondTaskTimeTableItem.EndDateTime,
                EndDateTimeRange = new DateTime(2025, 09, 26, 19, 20, 00),
            };
            Assert.IsTrue(expectedTask.Equals(task));
        }
    }
}
