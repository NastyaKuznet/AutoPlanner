using AutoPlannerApi.Controllers.Model;
using AutoPlannerApi.Domain.UserDomain.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AutoPlannerApi.Controllers
{
    [ApiController]
    [Route("telegram")]
    public class TelegramController : ControllerBase
    {
        private readonly ITelegramLinkingService _linkingService;

        public TelegramController(ITelegramLinkingService linkingService)
        {
            _linkingService = linkingService;
        }

        [HttpPost("generate-code")]
        public async Task<IActionResult> GenerateLinkingCode([FromBody] GenerateLinkingCodeRequest request)
        {
            var code = await _linkingService.GenerateLinkingCode(request.UserId);
            var telegramLink = $"https://t.me/autoplannernotifierbot?start={code}";

            return Ok(new GenerateLinkingCodeResponse
            {
                Code = code,
                TelegramLink = telegramLink
            });
        }

        [HttpPost("link")]
        public async Task<IActionResult> LinkTelegram([FromBody] LinkTelegramRequest request)
        {
            var result = await _linkingService.LinkUserToTelegram(request.Code, request.ChatId);

            if (result.Success)
            {
                return Ok(new LinkTelegramResponse
                {
                    Success = true,
                    UserId = result.UserId,
                    UserName = result.UserName
                });
            }

            return BadRequest(new LinkTelegramResponse
            {
                Success = false,
                ErrorMessage = result.ErrorMessage
            });
        }

        [HttpGet("tasks/{userId}")]
        public async Task<IActionResult> GetUserTasksForNotification(int userId)
        {
            var tasks = await _linkingService.GetUserTasksForNotification(userId);
            return Ok(tasks);
        }
    }
}
