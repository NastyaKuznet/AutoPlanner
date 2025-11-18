using AutoPlannerApi.Data.PlanningTaskData.Interface;
using AutoPlannerApi.Data.PlanningTaskData.Model;
using AutoPlannerApi.Data.PlanningTaskData.Model.Answer;
using AutoPlannerApi.Data.PlanningTaskData.Model.Answer.AnswerStatus;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AutoPlannerApi.Data.PlanningTaskData.Realization
{
    public class PlanningTaskPostgresRepository : IPlanningTaskDatabaseRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<PlanningTaskPostgresRepository> _logger;

        public PlanningTaskPostgresRepository(string connectionString, ILogger<PlanningTaskPostgresRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<AddPlanningTaskDatabaseAnswer> Add(PlanningTaskDatabase planningTask)
        {
            _logger.LogInformation("Начало добавления задачи планирования. UserId: {UserId}, MyTaskId: {MyTaskId}",
                planningTask.UserId, planningTask.MyTaskId);
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    INSERT INTO planning_tasks (
                        user_id, my_task_id, name, description, priority, start_date_time, 
                        end_date_time, duration, count_from, is_complete, complete_date_time,
                        start_date_time_range, end_date_time_range, rule_one_task,
                        start_date_time_rule_one_task, end_date_time_rule_one_task, rule_two_task,
                        time_position_regarding_task_id, second_task_id, relation_range_id, date_time_range
                    ) VALUES (
                        @userId, @myTaskId, @name, @description, @priority, @startDateTime,
                        @endDateTime, @duration, @countFrom, @isComplete, @completeDateTime,
                        @startDateTimeRange, @endDateTimeRange, @ruleOneTask,
                        @startDateTimeRuleOneTask, @endDateTimeRuleOneTask, @ruleTwoTask,
                        @timePositionRegardingTaskId, @secondTaskId, @relationRangeId, @dateTimeRange
                    )";

                using var command = new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("userId", planningTask.UserId);
                command.Parameters.AddWithValue("myTaskId", planningTask.MyTaskId);
                command.Parameters.AddWithValue("name", planningTask.Name ?? "");
                command.Parameters.AddWithValue("description", planningTask.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("priority", planningTask.Priority);
                command.Parameters.AddWithValue("startDateTime", planningTask.StartDateTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("endDateTime", planningTask.EndDateTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("duration", planningTask.Duration ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("countFrom", planningTask.CountFrom);
                command.Parameters.AddWithValue("isComplete", planningTask.IsComplete);
                command.Parameters.AddWithValue("completeDateTime", planningTask.CompleteDateTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("startDateTimeRange", planningTask.StartDateTimeRange ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("endDateTimeRange", planningTask.EndDateTimeRange ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("ruleOneTask", planningTask.RuleOneTask);
                command.Parameters.AddWithValue("startDateTimeRuleOneTask", planningTask.StartDateTimeRuleOneTask ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("endDateTimeRuleOneTask", planningTask.EndDateTimeRuleOneTask ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("ruleTwoTask", planningTask.RuleTwoTask);
                command.Parameters.AddWithValue("timePositionRegardingTaskId", planningTask.TimePositionRegardingTaskId);
                command.Parameters.AddWithValue("secondTaskId", planningTask.SecondTaskId);
                command.Parameters.AddWithValue("relationRangeId", planningTask.RelationRangeId);
                command.Parameters.AddWithValue("dateTimeRange", planningTask.DateTimeRange ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Задача планирования успешно добавлена. UserId: {UserId}, MyTaskId: {MyTaskId}",
                    planningTask.UserId, planningTask.MyTaskId);

                return new AddPlanningTaskDatabaseAnswer
                {
                    Status = new AddPlanningTaskDatabaseAnswerStatus { Status = AddPlanningTaskDatabaseAnswerStatus.Good }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении задачи планирования. UserId: {UserId}, MyTaskId: {MyTaskId}",
                    planningTask.UserId, planningTask.MyTaskId);

                return new AddPlanningTaskDatabaseAnswer
                {
                    Status = new AddPlanningTaskDatabaseAnswerStatus { Status = AddPlanningTaskDatabaseAnswerStatus.Bad }
                };
            }
        }

        public async Task<DeletePlanningTaskDatabaseAnswer> DeleteByUserId(int userId)
        {
            _logger.LogInformation("Начало удаления задач планирования по UserId: {UserId}", userId);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "DELETE FROM planning_tasks WHERE user_id = @userId";
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("userId", userId);

                await command.ExecuteNonQueryAsync();

                return new DeletePlanningTaskDatabaseAnswer
                {
                    Status = new DeletePlanningTaskDatabaseAnswerStatus { Status = DeletePlanningTaskDatabaseAnswerStatus.Good }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении задач планирования по UserId: {UserId}",
                    userId);

                return new DeletePlanningTaskDatabaseAnswer
                {
                    Status = new DeletePlanningTaskDatabaseAnswerStatus { Status = DeletePlanningTaskDatabaseAnswerStatus.Bad }
                };
            }
        }

        public async Task<GetPlanningTasksByUserIdDatabaseAnswer> Get(int userId)
        {
            _logger.LogInformation("Начало получения задач планирования по UserId: {UserId}", userId);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT * FROM planning_tasks WHERE user_id = @userId";
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("userId", userId);

                using var reader = await command.ExecuteReaderAsync();
                var planningTasks = new List<PlanningTaskDatabase>();

                while (await reader.ReadAsync())
                {
                    TimeSpan? duration = null;
                    int durationOrdinal = reader.GetOrdinal("duration");
                    if (!reader.IsDBNull(durationOrdinal))
                    {
                        // Читаем напрямую как TimeSpan, а не как строку
                        duration = reader.GetTimeSpan(durationOrdinal);
                    }

                    TimeSpan? dateTimeRange = null;
                    if (!reader.IsDBNull("date_time_range"))
                    {
                        var dateTimeRangeString = reader.GetString("date_time_range");
                        if (TimeSpan.TryParse(dateTimeRangeString, out TimeSpan parsedDateTimeRange))
                        {
                            dateTimeRange = parsedDateTimeRange;
                        }
                    }

                    planningTasks.Add(new PlanningTaskDatabase(
                        userId: reader.GetInt32("user_id"),
                        myTaskId: reader.GetInt32("my_task_id"),
                        name: reader.GetString("name"),
                        description: reader.IsDBNull("description") ? null : reader.GetString("description"),
                        priority: reader.GetInt32("priority"),
                        startDateTime: reader.IsDBNull("start_date_time") ? null : reader.GetDateTime("start_date_time"),
                        endDateTime: reader.IsDBNull("end_date_time") ? null : reader.GetDateTime("end_date_time"),
                        duration: duration,
                        countFrom: reader.GetInt32("count_from"),
                        isComplete: reader.GetBoolean("is_complete"),
                        completeDateTime: reader.IsDBNull("complete_date_time") ? null : reader.GetDateTime("complete_date_time"),
                        startDateTimeRange: reader.IsDBNull("start_date_time_range") ? null : reader.GetDateTime("start_date_time_range"),
                        endDateTimeRange: reader.IsDBNull("end_date_time_range") ? null : reader.GetDateTime("end_date_time_range"),
                        ruleOneTask: reader.GetBoolean("rule_one_task"),
                        startDateTimeRuleOneTask: reader.IsDBNull("start_date_time_rule_one_task") ? null : reader.GetDateTime("start_date_time_rule_one_task"),
                        endDateTimeRuleOneTask: reader.IsDBNull("end_date_time_rule_one_task") ? null : reader.GetDateTime("end_date_time_rule_one_task"),
                        ruleTwoTask: reader.GetBoolean("rule_two_task"),
                        timePositionRegardingTaskId: reader.GetInt32("time_position_regarding_task_id"),
                        secondTaskId: reader.GetInt32("second_task_id"),
                        relationRangeId: reader.GetInt32("relation_range_id"),
                        dateTimeRange: dateTimeRange
                    ));
                }

                return new GetPlanningTasksByUserIdDatabaseAnswer
                {
                    Status = new GetPlanningTasksByUserIdDatabaseAnswerStatus { Status = GetPlanningTasksByUserIdDatabaseAnswerStatus.Good },
                    PlanningTasks = planningTasks
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении задач планирования по UserId: {UserId}",
                    userId);

                return new GetPlanningTasksByUserIdDatabaseAnswer
                {
                    Status = new GetPlanningTasksByUserIdDatabaseAnswerStatus { Status = GetPlanningTasksByUserIdDatabaseAnswerStatus.Bad },
                    PlanningTasks = new List<PlanningTaskDatabase>()
                };
            }
        }
    }
}
