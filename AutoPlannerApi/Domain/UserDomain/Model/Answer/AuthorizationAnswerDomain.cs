using AutoPlannerApi.Domain.UserDomain.Model.AnswerStatus;

namespace AutoPlannerApi.Domain.UserDomain.Model.Answer
{
    public class AuthorizationAnswerDomain
    {
        public AuthorizationAnswerStatusDomain Status { get; }

        public int UserId { get; }

        public AuthorizationAnswerDomain(AuthorizationAnswerStatusDomain status, int userId)
        {
            Status = status;
            UserId = userId;
        }
    }
}
