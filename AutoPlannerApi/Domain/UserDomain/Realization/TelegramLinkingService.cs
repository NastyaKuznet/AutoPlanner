using AutoPlannerApi.Data.UserData.Interface;
using AutoPlannerApi.Domain.TaskDomain.Interface;
using AutoPlannerApi.Domain.TimeTableDomain.Interface;
using AutoPlannerApi.Domain.TimeTableDomain.Model;
using AutoPlannerApi.Domain.UserDomain.Interface;
using AutoPlannerApi.Domain.UserDomain.Model;

namespace AutoPlannerApi.Domain.UserDomain.Realization
{
    public class TelegramLinkingService : ITelegramLinkingService
    {
        private readonly IUserDatabaseRepository _userRepository;
        private readonly ITimeTableItemService _timeTableService;
        private readonly Dictionary<string, LinkingCodeDomain> _linkingCodes;

        public TelegramLinkingService(
            IUserDatabaseRepository userRepository,
            ITimeTableItemService timeTableService)
        {
            _userRepository = userRepository;
            _timeTableService = timeTableService;
            _linkingCodes = new Dictionary<string, LinkingCodeDomain>();
        }

        public async Task<string> GenerateLinkingCode(int userId)
        {
            var code = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var expiresAt = DateTime.Now.AddMinutes(10);

            _linkingCodes[code] = new LinkingCodeDomain(
                code, userId, DateTime.UtcNow, expiresAt);

            CleanExpiredCodes();
            return code;
        }

        public async Task<LinkingResultDomain> LinkUserToTelegram(string code, long chatId)
        {
            CleanExpiredCodes();

            if (!_linkingCodes.TryGetValue(code, out var codeInfo))
            {
                return new LinkingResultDomain { Success = false, ErrorMessage = "Неверный код привязки" };
            }

            if (codeInfo.ExpiresAt < DateTime.Now)
            {
                _linkingCodes.Remove(code);
                return new LinkingResultDomain { Success = false, ErrorMessage = "Срок действия кода истек" };
            }

            var user = await _userRepository.GetUserById(codeInfo.UserId);
            if (user == null)
            {
                return new LinkingResultDomain
                {
                    Success = false,
                    ErrorMessage = "Пользователь не найден в базе данных."
                };
            }

            var updated = await _userRepository.UpdateUserTelegramChatId(codeInfo.UserId, chatId);
            if (!updated)
            {
                return new LinkingResultDomain
                {
                    Success = false,
                    ErrorMessage = "Не удалось обновить данные пользователя."
                };
            }

            _linkingCodes.Remove(code);

            return new LinkingResultDomain
            {
                Success = true,
                UserId = codeInfo.UserId,
                UserName = user.Nickname,
                ChatId = chatId
            };
        }

        public async Task<List<TimeTableItemDomain>> GetUserTasksForNotification(int userId)
        {
            var timeTableResult = await _timeTableService.Get(userId);
            if (timeTableResult.Item1.Status != Domain.TimeTableDomain.Model.Answer.AnswerStatus.GetTTByUserIdAnswerStatusDomain.Good)
            {
                return new List<TimeTableItemDomain>();
            }

            TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Yekaterinburg");

            return timeTableResult.Item2
                .Where(t => !t.IsComplete)
                .Where(t =>
                {
                        DateTime taskTimeUtc = TimeZoneInfo.ConvertTimeToUtc(
                        DateTime.SpecifyKind(t.StartDateTime, DateTimeKind.Unspecified), moscowTimeZone);
                        return taskTimeUtc > DateTime.UtcNow;
                }).ToList();
        }

        private void CleanExpiredCodes()
        {
            var expiredCodes = _linkingCodes
                .Where(kv => kv.Value.ExpiresAt < DateTime.UtcNow)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var code in expiredCodes)
            {
                _linkingCodes.Remove(code);
            }
        }
    }
}
