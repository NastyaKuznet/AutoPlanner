using AutoPlannerApi.Controllers.Model;
using AutoPlannerApi.Domain.TaskDomain.Interface;
using AutoPlannerApi.Domain.TaskDomain.Model;
using AutoPlannerApi.Domain.TaskDomain.Model.AnswerStatus;
using Microsoft.AspNetCore.Mvc;

namespace AutoPlannerApi.Controllers
{
    [ApiController]
    [Route("task")]
    public class TaskController : ControllerBase
    {
        private ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] TaskForAddControl taskForAdd, int userId)
        {
            var status = await _taskService.Add(new TaskForAddDomain(
                taskForAdd.Name,
                taskForAdd.Description,
                taskForAdd.CreatedDate,
                taskForAdd.Priority,
                taskForAdd.StartDateTime,
                taskForAdd.EndDateTime,
                taskForAdd.Duration,
                taskForAdd.IsRepit,
                taskForAdd.RepitTime,
                taskForAdd.IsRepitFromStart,
                taskForAdd.CountRepit,
                taskForAdd.StartDateTimeRepit,
                taskForAdd.EndDateTimeRepit,
                taskForAdd.RuleOneTask,
                taskForAdd.StartDateTimeRuleOneTask,
                taskForAdd.EndDateTimeRuleOneTask,
                taskForAdd.RuleTwoTask,
                taskForAdd.TimePositionRegardingTaskId,
                taskForAdd.SecondTaskId,
                taskForAdd.RelationRangeId,
                taskForAdd.DateTimeRange,
                false,
                null),
                userId);

            if (status.Status == AddTaskAnswerStatusDomain.Good)
            {
                return Ok();
            }
            if (status.Status == AddTaskAnswerStatusDomain.UserNotExists)
            {
                return BadRequest("User with this id not found.");
            }
            if (status.Status == AddTaskAnswerStatusDomain.Bad)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Task wasn`t added.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Unknow error.");
        }

        [HttpPut()]
        public async Task<IActionResult> Edit([FromForm] TaskForEditControl taskforEdit)
        {
            var status = await _taskService.Edit(new TaskForEditDomain(
                taskforEdit.Id,
                taskforEdit.Name,
                taskforEdit.Description,
                taskforEdit.Priority,
                taskforEdit.StartDateTime,
                taskforEdit.EndDateTime,
                taskforEdit.Duration,
                taskforEdit.IsRepit,
                taskforEdit.RepitTime,
                taskforEdit.IsRepitFromStart,
                taskforEdit.CountRepit,
                taskforEdit.StartDateTimeRepit,
                taskforEdit.EndDateTimeRepit,
                taskforEdit.RuleOneTask,
                taskforEdit.StartDateTimeRuleOneTask,
                taskforEdit.EndDateTimeRuleOneTask,
                taskforEdit.RuleTwoTask,
                taskforEdit.TimePositionRegardingTaskId,
                taskforEdit.SecondTaskId,
                taskforEdit.RelationRangeId,
                taskforEdit.DateTimeRange,
                taskforEdit.IsComplete,
                taskforEdit.CompleteDateTime));
            if (status.Status == TaskForEditAnswerStatusDomain.Good)
            {
                return Ok();
            }
            if (status.Status == TaskForEditAnswerStatusDomain.TaskNotExist)
            {
                return BadRequest("Task with this id not found.");
            }
            if (status.Status == TaskForEditAnswerStatusDomain.NotDeleteFromTimeTable)
            {
                return StatusCode(StatusCodes.Status200OK, "Old task not delete from time table. Call administrator.");
            }
            if (status.Status == TaskForEditAnswerStatusDomain.Bad)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Task wasn`t edit.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Unknow error.");
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> Delete([FromRoute] int taskId)
        {
            var status = await _taskService.Delete(taskId);
            if (status.Status == DeleteTaskAnswerStatusDomain.Good)
            {
                return Ok();
            }
            if (status.Status == DeleteTaskAnswerStatusDomain.TaskNotExist)
            {
                return BadRequest("Task with this id not found.");
            }
            if (status.Status == DeleteTaskAnswerStatusDomain.Bad)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Task wasn`t delete.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Unknow error.");
        }

        [HttpPut("complete/{taskId}")]
        public async Task<IActionResult> SetComplete([FromRoute] int taskId)
        {
            var status = await _taskService.SetComplete(taskId);
            if (status.Status == SetCompleteAnswerStatusDomain.Good)
            {
                return Ok();
            }
            if (status.Status == SetCompleteAnswerStatusDomain.TaskNotExist)
            {
                return BadRequest("Task with this id not found.");
            }
            if (status.Status == SetCompleteAnswerStatusDomain.TimeTableNotExist)
            {
                return BadRequest("Task not find in time table. Call administrator.");
            }
            if (status.Status == SetCompleteAnswerStatusDomain.Bad)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Task wasn`t set complete.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Unknow error.");
        }

        [HttpPut("complete/repit")]
        public async Task<IActionResult> SetCompleteForRepit(int taskId, int countFrom)
        {
            var status = await _taskService.SetCompleteForRepit(taskId, countFrom);
            if (status.Status == SetCompleteForRepitTaskStatusDomain.Good)
            {
                return Ok();
            }
            if (status.Status == SetCompleteForRepitTaskStatusDomain.TaskNotExist)
            {
                return BadRequest("Task with this id not found.");
            }
            if (status.Status == SetCompleteForRepitTaskStatusDomain.Bad)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Task wasn`t set complete.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Unknow error.");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get([FromRoute] int userId)
        {
            var status = await _taskService.Get(userId);
            if (status.Status.Status == GetByUserIdAnswerStatusDomain.Good)
            {
                return Ok(status.Tasks);
            }
            if (status.Status.Status == GetByUserIdAnswerStatusDomain.UserNotExist)
            {
                return BadRequest("user with id not found.");
            }
            if (status.Status.Status == GetByUserIdAnswerStatusDomain.Bad)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Tasks didn`t have.");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Unknow error.");
        }

        [HttpGet("/id/{taskId}")]
        public async Task<IActionResult> GetById([FromRoute] int taskId)
        {
            var status = await _taskService.GetById(taskId);
            return Ok(status);
        }
    }
}
