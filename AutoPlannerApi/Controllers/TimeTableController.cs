using AutoPlannerApi.Controllers.Model;
using AutoPlannerApi.Domain.TimeTableDomain.Interface;
using AutoPlannerApi.Domain.TimeTableDomain.Model.Answer.AnswerStatus;
using Microsoft.AspNetCore.Mvc;

namespace AutoPlannerApi.Controllers
{
    [ApiController]
    [Route("time-table")]
    public class TimeTableController : ControllerBase
    {
        private ITimeTableItemService _timeTableItemService;

        public TimeTableController(ITimeTableItemService timeTableItemService)
        {
            _timeTableItemService = timeTableItemService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get([FromRoute]int userId)
        {
            var result = await _timeTableItemService.Get(userId);
            if (result.Item1.Status == GetTTByUserIdAnswerStatusDomain.UserNotExist)
            {
                return BadRequest("User with this id not found.");
            }
            if (result.Item1.Status == GetTTByUserIdAnswerStatusDomain.Bad)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "TimeTableItems wasn't get.");
            }
            if (result.Item1.Status == GetTTByUserIdAnswerStatusDomain.Good)
            {
                return Ok(result.Item2);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Unknow error.");
        }

        [HttpPost()]
        public async Task<IActionResult> Recreate(int userId, DateTime startTimeTable, DateTime endDateTime)
        {
            var result = await _timeTableItemService.Recreate(userId, startTimeTable, endDateTime);
            if (result.Status.Status == GetTTByUserIdAnswerStatusDomain.UserNotExist)
            {
                return BadRequest("User with this id not found.");
            }
            if (result.Status.Status == GetTTByUserIdAnswerStatusDomain.Bad)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "TimeTableItems wasn't recreate.");
            }
            if (result.Status.Status == GetTTByUserIdAnswerStatusDomain.Good)
            {
                return Ok(new RecreateTimeTableAnswer(result.TimeTableItems, result.PenaltyTasks));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Unknow error.");
        }
    }
}
