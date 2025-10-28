using AutoPlannerApi.Data.Common.Model;
using AutoPlannerApi.Data.TimeTableData.Interface;
using AutoPlannerApi.Data.TimeTableData.Model;
using AutoPlannerApi.Data.TimeTableData.Model.Answer;
using AutoPlannerApi.Data.TimeTableData.Model.Answer.AnswerStatus;

namespace AutoPlannerApi.Data.TimeTableData.Realization
{
    public class TimeTableItemRamRepository : ITimeTableItemDatabaseRepository
    {
        private List<TimeTableItemDatabase> _timeTableItems = new List<TimeTableItemDatabase>();

        public Task<AddTimeTableItemAnswerStatusDatabase> Add(TimeTableItemForAddDatabase timeTableItemForAdd)
        {
            // validation for example
            _timeTableItems.Add(new TimeTableItemDatabase(
                timeTableItemForAdd.MyTaskId,
                timeTableItemForAdd.UserId,
                timeTableItemForAdd.CountFrom,
                timeTableItemForAdd.Name,
                timeTableItemForAdd.StartDateTime,
                timeTableItemForAdd.EndDateTime,
                timeTableItemForAdd.IsComplete,
                timeTableItemForAdd.CompleteDateTime));
            return Task.FromResult(new AddTimeTableItemAnswerStatusDatabase()
            {
                Status = AddTimeTableItemAnswerStatusDatabase.Good,
            });
        }

        public Task<DeleteTaskFromTimeTableAnswerStatusDatabase> Delete(int taskId)
        {
            TimeTableItemDatabase deleteTimeTableItem = null;
            foreach (var item in _timeTableItems)
            {
                if (item.MyTaskId == taskId)
                {
                    deleteTimeTableItem = item;
                }
            }
            if (deleteTimeTableItem == null)
            {
                return Task.FromResult(new DeleteTaskFromTimeTableAnswerStatusDatabase()
                {
                    Status = DeleteTaskFromTimeTableAnswerStatusDatabase.TaskIsNotExist,
                });
            }
            _timeTableItems.Remove(deleteTimeTableItem);
            return Task.FromResult(new DeleteTaskFromTimeTableAnswerStatusDatabase()
            {
                Status = DeleteTaskFromTimeTableAnswerStatusDatabase.Good,
            });
        }

        public Task<GetByIdAnswerAnswerData> Get(int userId)
        {
            var userTimeTableItems = new List<TimeTableItemDatabase>();
            foreach (var timeTableItem in _timeTableItems)
            {
                if (timeTableItem.UserId == userId)
                {
                    userTimeTableItems.Add(timeTableItem);
                }
            }
            return Task.FromResult(new GetByIdAnswerAnswerData(
                new ClassicAnswerStatus() 
                {
                    Status = ClassicAnswerStatus.Good,
                },
                userTimeTableItems));
        }

        public Task<SetCompleteTimeTableItemAnswerStatusDatabase> SetComplete(int taskId)
        {
            var flag = false;
            foreach (var item in _timeTableItems)
            {
                if(item.MyTaskId == taskId)
                {
                    item.IsComplete = true;
                    item.CompleteDateTime = DateTime.Now;
                    flag = true;
                }
            }
            if (!flag)
            {
                return Task.FromResult(new SetCompleteTimeTableItemAnswerStatusDatabase() 
                {
                    Status = SetCompleteTimeTableItemAnswerStatusDatabase.TimeTableItemNotExist,
                });
            }
            return Task.FromResult(new SetCompleteTimeTableItemAnswerStatusDatabase()
            {
                Status = SetCompleteTimeTableItemAnswerStatusDatabase.Good,
            });
        }
    }
}
