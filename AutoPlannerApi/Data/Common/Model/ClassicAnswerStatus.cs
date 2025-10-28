namespace AutoPlannerApi.Data.Common.Model
{
    public class ClassicAnswerStatus
    {
        public static readonly int Good = 0;
        public static readonly int Bad = 1;

        public int Status { get; set; } = Good; 
    }
}
