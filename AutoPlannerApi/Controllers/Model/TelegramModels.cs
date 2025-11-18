namespace AutoPlannerApi.Controllers.Model
{
    public class GenerateLinkingCodeRequest
    {
        public int UserId { get; set; }
    }

    public class GenerateLinkingCodeResponse
    {
        public string Code { get; set; }
        public string TelegramLink { get; set; }
    }

    public class LinkTelegramRequest
    {
        public string Code { get; set; }
        public long ChatId { get; set; }
    }

    public class LinkTelegramResponse
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
