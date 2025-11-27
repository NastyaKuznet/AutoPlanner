namespace AutoPlannerApi.Data.NotificationData.Model
{
    public class SentNotification
    {
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
