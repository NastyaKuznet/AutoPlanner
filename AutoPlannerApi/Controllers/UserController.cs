using AutoPlannerApi.Domain.UserDomain.Interface;
using AutoPlannerApi.Domain.UserDomain.Model;
using AutoPlannerApi.Domain.UserDomain.Model.AnswerStatus;
using Microsoft.AspNetCore.Mvc;

namespace AutoPlannerApi.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService) 
        {
            _userService = userService;
        }

        [HttpGet("registrate")]
        public async Task<IActionResult> Registrate(string nickname, string password)
        {
            var status = await _userService.Registrate(
                new UserForRegistrationAndAuthorizationDomain(
                    nickname,
                    password));
            if (status.Item1.Status == RegistrationAnswerStatusDomain.Good)
            {
                return Ok(status.Item2);
            }
            return BadRequest();
        }

        [HttpGet("auth")]
        public async Task<IActionResult> Authorization(string nickname, string password)
        {
            var status = await _userService.Authorization(
                new UserForRegistrationAndAuthorizationDomain(
                    nickname,
                    password));
            if (status.Status.Status == AuthorizationAnswerStatusDomain.Good)
            {
                return Ok(status.UserId);
            }
            if(status.Status.Status == AuthorizationAnswerStatusDomain.NicknameNotExist || status.Status.Status == AuthorizationAnswerStatusDomain.PasswordNotCorrect)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "User with nickname and password not exist.");
            }
            return BadRequest();
        }
    }
}
