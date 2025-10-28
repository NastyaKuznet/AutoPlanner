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
                new UserForRegistrationDomain(
                    nickname,
                    password));
            if (status.Status == RegistrationAnswerStatusDomain.Good)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
