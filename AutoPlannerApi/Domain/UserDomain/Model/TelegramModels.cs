namespace AutoPlannerApi.Domain.UserDomain.Model
{
    public class LinkingCodeDomain
    {
        public string Code { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public LinkingCodeDomain(string code, int userId, DateTime createdAt, DateTime expiresAt)
        {
            Code = code;
            UserId = userId;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
        }
    }

    public class LinkingResultDomain
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ErrorMessage { get; set; }
        public long ChatId { get; set; }
    }
}
