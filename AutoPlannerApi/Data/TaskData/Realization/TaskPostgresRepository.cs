using AutoPlannerApi.Data.Common.Model;
using AutoPlannerApi.Data.TaskData.Interface;
using AutoPlannerApi.Data.TaskData.Model;
using AutoPlannerApi.Data.TaskData.Model.Answer;
using AutoPlannerApi.Data.TaskData.Model.Answer.AnswerStatus;
using Npgsql;
using System.Data;
using System.Reflection;

namespace AutoPlannerApi.Data.TaskData.Realization
{
    public class TaskPostgresRepository : ITaskDatabaseRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TaskPostgresRepository> _logger;

        public TaskPostgresRepository(string connectionString, ILogger<TaskPostgresRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<AddTaskAnswerStatusData> Add(TaskForAddData taskForAdd, int userId)
        {
            _logger.LogInformation("Начало добавления задачи. UserId: {UserId}, Name: {TaskName}",
                userId, taskForAdd.Name);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                INSERT INTO tasks (
                    user_id, name, description, created_date, priority, start_date_time, 
                    end_date_time, duration, is_repit, repit_time, is_repit_from_start, 
                    count_repit, start_date_time_repit, end_date_time_repit, rule_one_task,
                    start_date_time_rule_one_task, end_date_time_rule_one_task, rule_two_task,
                    time_position_regarding_task_id, second_task_id, relation_range_id,
                    date_time_range, is_complete, complete_date_time
                ) VALUES (
                    @userId, @name, @description, @createdDate, @priority, @startDateTime,
                    @endDateTime, @duration, @isRepit, @repitTime, @isRepitFromStart,
                    @countRepit, @startDateTimeRepit, @endDateTimeRepit, @ruleOneTask,
                    @startDateTimeRuleOneTask, @endDateTimeRuleOneTask, @ruleTwoTask,
                    @timePositionRegardingTaskId, @secondTaskId, @relationRangeId,
                    @dateTimeRange, @isComplete, @completeDateTime
                ) RETURNING id";

                using var command = new NpgsqlCommand(sql, connection);


                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("name", taskForAdd.Name ?? "");
                command.Parameters.AddWithValue("description", taskForAdd.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("createdDate", taskForAdd.CreatedDate);
                command.Parameters.AddWithValue("priority", taskForAdd.Priority);
                command.Parameters.AddWithValue("isRepit", taskForAdd.IsRepit);
                command.Parameters.AddWithValue("isRepitFromStart", taskForAdd.IsRepitFromStart);
                command.Parameters.AddWithValue("countRepit", taskForAdd.CountRepit);
                command.Parameters.AddWithValue("ruleOneTask", taskForAdd.RuleOneTask);
                command.Parameters.AddWithValue("ruleTwoTask", taskForAdd.RuleTwoTask);
                command.Parameters.AddWithValue("timePositionRegardingTaskId", taskForAdd.TimePositionRegardingTaskId);
                command.Parameters.AddWithValue("relationRangeId", taskForAdd.RelationRangeId);
                command.Parameters.AddWithValue("isComplete", taskForAdd.IsComplete);

                command.Parameters.AddWithValue("startDateTime", taskForAdd.StartDateTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("endDateTime", taskForAdd.EndDateTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("duration", taskForAdd.Duration ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("repitTime", taskForAdd.RepitTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("startDateTimeRepit", taskForAdd.StartDateTimeRepit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("endDateTimeRepit", taskForAdd.EndDateTimeRepit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("startDateTimeRuleOneTask", taskForAdd.StartDateTimeRuleOneTask ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("endDateTimeRuleOneTask", taskForAdd.EndDateTimeRuleOneTask ?? (object)DBNull.Value);

                var secondTaskIdValue = taskForAdd.SecondTaskId == 0 ? (object)DBNull.Value : taskForAdd.SecondTaskId;
                command.Parameters.AddWithValue("secondTaskId", secondTaskIdValue);

                command.Parameters.AddWithValue("dateTimeRange", taskForAdd.DateTimeRange ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("completeDateTime", taskForAdd.CompleteDateTime ?? (object)DBNull.Value);

                var newId = await command.ExecuteScalarAsync();

                return new AddTaskAnswerStatusData { Status = AddTaskAnswerStatusData.Good };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении задачи. UserId: {UserId}, Name: {TaskName}",
                    userId, taskForAdd.Name);

                return new AddTaskAnswerStatusData { Status = AddTaskAnswerStatusData.Bad };
            }
        }

        public async Task<DeleteTaskAnswerStatusDatabase> Delete(int taskId)
        {
            _logger.LogInformation("Начало удаления задачи. TaskId: {TaskId}", taskId);
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "DELETE FROM tasks WHERE id = @id";
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("id", taskId);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new DeleteTaskAnswerStatusDatabase { Status = DeleteTaskAnswerStatusDatabase.TaskNotExist };
                }

                return new DeleteTaskAnswerStatusDatabase { Status = DeleteTaskAnswerStatusDatabase.Good };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении задачи. TaskId: {TaskId}",
                    taskId);

                return new DeleteTaskAnswerStatusDatabase { Status = DeleteTaskAnswerStatusDatabase.Bad };
            }
        }

        public async Task<TaskForEditAnswerStatusData> Edit(TaskForEditDatabase taskForEdit)
        {
            _logger.LogInformation("Начало редактирования задачи. TaskId: {TaskId}, Name: {TaskName}",
                taskForEdit.Id, taskForEdit.Name);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    UPDATE tasks SET
                        name = @name,
                        description = @description,
                        priority = @priority,
                        start_date_time = @startDateTime,
                        end_date_time = @endDateTime,
                        duration = @duration,
                        is_repit = @isRepit,
                        repit_time = @repitTime,
                        is_repit_from_start = @isRepitFromStart,
                        count_repit = @countRepit,
                        start_date_time_repit = @startDateTimeRepit,
                        end_date_time_repit = @endDateTimeRepit,
                        rule_one_task = @ruleOneTask,
                        start_date_time_rule_one_task = @startDateTimeRuleOneTask,
                        end_date_time_rule_one_task = @endDateTimeRuleOneTask,
                        rule_two_task = @ruleTwoTask,
                        time_position_regarding_task_id = @timePositionRegardingTaskId,
                        second_task_id = @secondTaskId,
                        relation_range_id = @relationRangeId,
                        date_time_range = @dateTimeRange,
                        is_complete = @isComplete,
                        complete_date_time = @completeDateTime
                    WHERE id = @id";

                using var command = new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("id", taskForEdit.Id);
                command.Parameters.AddWithValue("name", taskForEdit.Name);
                command.Parameters.AddWithValue("description", taskForEdit.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("priority", taskForEdit.Priority);
                command.Parameters.AddWithValue("startDateTime", taskForEdit.StartDateTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("endDateTime", taskForEdit.EndDateTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("duration", taskForEdit.Duration ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("isRepit", taskForEdit.IsRepit);
                command.Parameters.AddWithValue("repitTime", taskForEdit.RepitTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("isRepitFromStart", taskForEdit.IsRepitFromStart);
                command.Parameters.AddWithValue("countRepit", taskForEdit.CountRepit);
                command.Parameters.AddWithValue("startDateTimeRepit", taskForEdit.StartDateTimeRepit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("endDateTimeRepit", taskForEdit.EndDateTimeRepit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("ruleOneTask", taskForEdit.RuleOneTask);
                command.Parameters.AddWithValue("startDateTimeRuleOneTask", taskForEdit.StartDateTimeRuleOneTask ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("endDateTimeRuleOneTask", taskForEdit.EndDateTimeRuleOneTask ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("ruleTwoTask", taskForEdit.RuleTwoTask);
                command.Parameters.AddWithValue("timePositionRegardingTaskId", taskForEdit.TimePositionRegardingTaskId);
                command.Parameters.AddWithValue("secondTaskId", taskForEdit.SecondTaskId);
                command.Parameters.AddWithValue("relationRangeId", taskForEdit.RelationRangeId);
                command.Parameters.AddWithValue("dateTimeRange", taskForEdit.DateTimeRange ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("isComplete", taskForEdit.IsComplete);
                command.Parameters.AddWithValue("completeDateTime", taskForEdit.CompleteDateTime ?? (object)DBNull.Value);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    _logger.LogWarning("Задача не найдена при редактировании. TaskId: {TaskId}", taskForEdit.Id);
                    return new TaskForEditAnswerStatusData { Status = TaskForEditAnswerStatusData.TaskNotExist };
                }

                return new TaskForEditAnswerStatusData { Status = TaskForEditAnswerStatusData.Good };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при редактировании задачи. TaskId: {TaskId}",
                    taskForEdit.Id);

                return new TaskForEditAnswerStatusData { Status = TaskForEditAnswerStatusData.Bad };
            }
        }

        public async Task<GetByUserIdAnswerData> Get(int userId)
        {
            _logger.LogInformation("Начало получения задач по UserId: {UserId}", userId);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT * FROM tasks WHERE user_id = @userId";
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("userId", userId);

                using var reader = await command.ExecuteReaderAsync();
                var tasks = new List<TaskDatabase>();

                while (await reader.ReadAsync())
                {
                    var idIndex = reader.GetOrdinal("id");
                    var userIdIndex = reader.GetOrdinal("user_id");
                    var nameIndex = reader.GetOrdinal("name");
                    var descriptionIndex = reader.GetOrdinal("description");
                    var createdDateIndex = reader.GetOrdinal("created_date");
                    var priorityIndex = reader.GetOrdinal("priority");
                    var startDateTimeIndex = reader.GetOrdinal("start_date_time");
                    var endDateTimeIndex = reader.GetOrdinal("end_date_time");
                    var durationIndex = reader.GetOrdinal("duration");
                    var isRepitIndex = reader.GetOrdinal("is_repit");
                    var repitTimeIndex = reader.GetOrdinal("repit_time");
                    var isRepitFromStartIndex = reader.GetOrdinal("is_repit_from_start");
                    var countRepitIndex = reader.GetOrdinal("count_repit");
                    var startDateTimeRepitIndex = reader.GetOrdinal("start_date_time_repit");
                    var endDateTimeRepitIndex = reader.GetOrdinal("end_date_time_repit");
                    var ruleOneTaskIndex = reader.GetOrdinal("rule_one_task");
                    var startDateTimeRuleOneTaskIndex = reader.GetOrdinal("start_date_time_rule_one_task");
                    var endDateTimeRuleOneTaskIndex = reader.GetOrdinal("end_date_time_rule_one_task");
                    var ruleTwoTaskIndex = reader.GetOrdinal("rule_two_task");
                    var timePositionRegardingTaskIdIndex = reader.GetOrdinal("time_position_regarding_task_id");
                    var secondTaskIdIndex = reader.GetOrdinal("second_task_id");
                    var relationRangeIdIndex = reader.GetOrdinal("relation_range_id");
                    var dateTimeRangeIndex = reader.GetOrdinal("date_time_range");
                    var isCompleteIndex = reader.GetOrdinal("is_complete");
                    var completeDateTimeIndex = reader.GetOrdinal("complete_date_time");

                    tasks.Add(new TaskDatabase(
                        reader.GetInt32(idIndex),
                        reader.GetInt32(userIdIndex),
                        reader.GetString(nameIndex),
                        reader.IsDBNull(descriptionIndex) ? null : reader.GetString(descriptionIndex),
                        reader.GetDateTime(createdDateIndex),
                        reader.GetInt32(priorityIndex),
                        reader.IsDBNull(startDateTimeIndex) ? null : reader.GetDateTime(startDateTimeIndex),
                        reader.IsDBNull(endDateTimeIndex) ? null : reader.GetDateTime(endDateTimeIndex),
                        reader.IsDBNull(durationIndex) ? null : reader.GetTimeSpan(durationIndex),
                        reader.GetBoolean(isRepitIndex),
                        reader.IsDBNull(repitTimeIndex) ? null : reader.GetTimeSpan(repitTimeIndex),
                        reader.GetBoolean(isRepitFromStartIndex),
                        reader.GetInt32(countRepitIndex),
                        reader.IsDBNull(startDateTimeRepitIndex) ? null : reader.GetDateTime(startDateTimeRepitIndex),
                        reader.IsDBNull(endDateTimeRepitIndex) ? null : reader.GetDateTime(endDateTimeRepitIndex),
                        reader.GetBoolean(ruleOneTaskIndex),
                        reader.IsDBNull(startDateTimeRuleOneTaskIndex) ? null : reader.GetDateTime(startDateTimeRuleOneTaskIndex),
                        reader.IsDBNull(endDateTimeRuleOneTaskIndex) ? null : reader.GetDateTime(endDateTimeRuleOneTaskIndex),
                        reader.GetBoolean(ruleTwoTaskIndex),
                        reader.GetInt32(timePositionRegardingTaskIdIndex),
                        reader.IsDBNull(secondTaskIdIndex) ? 0 : reader.GetInt32(secondTaskIdIndex),
                        reader.GetInt32(relationRangeIdIndex),
                        reader.IsDBNull(dateTimeRangeIndex) ? null : reader.GetTimeSpan(dateTimeRangeIndex),
                        reader.GetBoolean(isCompleteIndex),
                        reader.IsDBNull(completeDateTimeIndex) ? null : reader.GetDateTime(completeDateTimeIndex)
                    ));
                }

                return new GetByUserIdAnswerData(
                    new ClassicAnswerStatus { Status = ClassicAnswerStatus.Good },
                    tasks
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении задач по UserId: {UserId}",
                    userId);

                return new GetByUserIdAnswerData(
                    new ClassicAnswerStatus { Status = ClassicAnswerStatus.Bad },
                    new List<TaskDatabase>()
                );
            }
        }

        public async Task<TaskDatabase> GetById(int id)
        {
            _logger.LogInformation("Начало получения задачи по Id: {TaskId}", id);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT * FROM tasks WHERE id = @id";
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("id", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    TimeSpan? duration = null;
                    if (!reader.IsDBNull("duration"))
                    {
                        var durationString = reader.GetString("duration");
                        if (TimeSpan.TryParse(durationString, out TimeSpan parsedDuration))
                        {
                            duration = parsedDuration;
                        }
                    }

                    TimeSpan? repitTime = null;
                    if (!reader.IsDBNull("repit_time"))
                    {
                        var repitTimeString = reader.GetString("repit_time");
                        if (TimeSpan.TryParse(repitTimeString, out TimeSpan parsedRepitTime))
                        {
                            repitTime = parsedRepitTime;
                        }
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

                    return new TaskDatabase(
                        reader.GetInt32("id"),
                        reader.GetInt32("user_id"),
                        reader.GetString("name"),
                        reader.IsDBNull("description") ? null : reader.GetString("description"),
                        reader.GetDateTime("created_date"),
                        reader.GetInt32("priority"),
                        reader.IsDBNull("start_date_time") ? null : reader.GetDateTime("start_date_time"),
                        reader.IsDBNull("end_date_time") ? null : reader.GetDateTime("end_date_time"),
                        duration,
                        reader.GetBoolean("is_repit"),
                        repitTime,
                        reader.GetBoolean("is_repit_from_start"),
                        reader.GetInt32("count_repit"),
                        reader.IsDBNull("start_date_time_repit") ? null : reader.GetDateTime("start_date_time_repit"),
                        reader.IsDBNull("end_date_time_repit") ? null : reader.GetDateTime("end_date_time_repit"),
                        reader.GetBoolean("rule_one_task"),
                        reader.IsDBNull("start_date_time_rule_one_task") ? null : reader.GetDateTime("start_date_time_rule_one_task"),
                        reader.IsDBNull("end_date_time_rule_one_task") ? null : reader.GetDateTime("end_date_time_rule_one_task"),
                        reader.GetBoolean("rule_two_task"),
                        reader.GetInt32("time_position_regarding_task_id"),
                        reader.IsDBNull("second_task_id") ? 0 : reader.GetInt32("second_task_id"),
                        reader.GetInt32("relation_range_id"),
                        dateTimeRange,
                        reader.GetBoolean("is_complete"),
                        reader.IsDBNull("complete_date_time") ? null : reader.GetDateTime("complete_date_time")
                    );
                }

                _logger.LogWarning("Задача не найдена по Id: {TaskId}", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении задачи по Id: {TaskId}",
                    id);

                return null;
            }
        }

        public async Task<SetCompleteAnswerStatusDatabase> SetComplete(int taskId)
        {
            _logger.LogInformation("Начало отметки задачи как выполненной. TaskId: {TaskId}", taskId);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    UPDATE tasks SET 
                        is_complete = true, 
                        complete_date_time = @completeDateTime 
                    WHERE id = @id";

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("id", taskId);
                command.Parameters.AddWithValue("completeDateTime", DateTime.Now);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new SetCompleteAnswerStatusDatabase { Status = SetCompleteAnswerStatusDatabase.TaskNotExist };
                }

                return new SetCompleteAnswerStatusDatabase { Status = SetCompleteAnswerStatusDatabase.Good };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отметке задачи как выполненной. TaskId: {TaskId}",
                    taskId);

                return new SetCompleteAnswerStatusDatabase { Status = SetCompleteAnswerStatusDatabase.Bad };
            }
        }
    }
}
