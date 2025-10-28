using AutoPlannerApi.Data.Common.Model;

namespace AutoPlannerApi.Data.TimeTableData.Model.Answer
{
    public class GetByIdAnswerAnswerData
    {
        public ClassicAnswerStatus Status { get; }

        public List<TimeTableItemDatabase> TimeTableItems { get; }

        public GetByIdAnswerAnswerData(
            ClassicAnswerStatus status, 
            List<TimeTableItemDatabase> timeTableItems)
        {
            Status = status;
            TimeTableItems = timeTableItems;
        }
    }
}
