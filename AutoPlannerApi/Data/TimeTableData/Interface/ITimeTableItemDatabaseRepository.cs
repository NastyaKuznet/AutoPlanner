using AutoPlannerApi.Data.TimeTableData.Model;
using AutoPlannerApi.Data.TimeTableData.Model.Answer;
using AutoPlannerApi.Data.TimeTableData.Model.Answer.AnswerStatus;

namespace AutoPlannerApi.Data.TimeTableData.Interface
{
    public interface ITimeTableItemDatabaseRepository
    {
        public Task<GetByIdAnswerAnswerData> Get(int userId);

        public Task<DeleteTaskFromTimeTableAnswerStatusDatabase> Delete(int taskId);

        public Task<AddTimeTableItemAnswerStatusDatabase> Add(TimeTableItemForAddDatabase timeTableItemForAdd);

        public Task<SetCompleteTimeTableItemAnswerStatusDatabase> SetComplete(int taskId);

        public Task<SetCompleteForRepitAnswerStatusDatabase> SetCompleteForRepit(int taskId, int countFrom);
    }
}
