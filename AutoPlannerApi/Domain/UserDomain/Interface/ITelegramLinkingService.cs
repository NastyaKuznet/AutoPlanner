using AutoPlannerApi.Domain.TimeTableDomain.Model;
using AutoPlannerApi.Domain.UserDomain.Model;

namespace AutoPlannerApi.Domain.UserDomain.Interface
{
    public interface ITelegramLinkingService
    {
        Task<string> GenerateLinkingCode(int userId);
        Task<LinkingResultDomain> LinkUserToTelegram(string code, long chatId);
        Task<List<TimeTableItemDomain>> GetUserTasksForNotification(int userId);
    }
}
