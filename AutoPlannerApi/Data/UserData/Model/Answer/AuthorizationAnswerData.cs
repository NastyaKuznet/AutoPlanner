using AutoPlannerApi.Data.UserData.Model.AnswerStatus;

namespace AutoPlannerApi.Data.UserData.Model.Answer
{
    public class AuthorizationAnswerData
    {
        public AuthorizationAnswerStatusData Status { get; }

        public int UserId { get; }

        public AuthorizationAnswerData(AuthorizationAnswerStatusData status, int userId)
        {
            Status = status;
            UserId = userId;
        }
    }
}
