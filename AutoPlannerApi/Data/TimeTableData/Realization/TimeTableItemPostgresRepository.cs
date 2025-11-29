using AutoPlannerApi.Data.Common.Model;
using AutoPlannerApi.Data.TimeTableData.Interface;
using AutoPlannerApi.Data.TimeTableData.Model;
using AutoPlannerApi.Data.TimeTableData.Model.Answer;
using AutoPlannerApi.Data.TimeTableData.Model.Answer.AnswerStatus;
using Npgsql;
using System.Data;
using System.Reflection;

namespace AutoPlannerApi.Data.TimeTableData.Realization
{
    public class TimeTableItemPostgresRepository : ITimeTableItemDatabaseRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TimeTableItemPostgresRepository> _logger;

        public TimeTableItemPostgresRepository(string connectionString, ILogger<TimeTableItemPostgresRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<AddTimeTableItemAnswerStatusDatabase> Add(TimeTableItemForAddDatabase timeTableItemForAdd)
        {
            _logger.LogInformation("Начало добавления элемента расписания. UserId: {UserId}, MyTaskId: {MyTaskId}, Name: {ItemName}",
                timeTableItemForAdd.UserId, timeTableItemForAdd.MyTaskId, timeTableItemForAdd.Name);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    INSERT INTO timetable_items (
                        my_task_id, user_id, count_from, name, priority, start_date_time, 
                        end_date_time, is_complete, complete_date_time
                    ) VALUES (
                        @myTaskId, @userId, @countFrom, @name, @priority, @startDateTime,
                        @endDateTime, @isComplete, @completeDateTime
                    )";

                using var command = new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("myTaskId", timeTableItemForAdd.MyTaskId);
                command.Parameters.AddWithValue("userId", timeTableItemForAdd.UserId);
                command.Parameters.AddWithValue("countFrom", timeTableItemForAdd.CountFrom);
                command.Parameters.AddWithValue("name", timeTableItemForAdd.Name);
                command.Parameters.AddWithValue("priority", timeTableItemForAdd.Priority);
                command.Parameters.AddWithValue("startDateTime", timeTableItemForAdd.StartDateTime);
                command.Parameters.AddWithValue("endDateTime", timeTableItemForAdd.EndDateTime);
                command.Parameters.AddWithValue("isComplete", timeTableItemForAdd.IsComplete);
                command.Parameters.AddWithValue("completeDateTime", timeTableItemForAdd.CompleteDateTime ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();

                return new AddTimeTableItemAnswerStatusDatabase { Status = AddTimeTableItemAnswerStatusDatabase.Good };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении элемента расписания. UserId: {UserId}, MyTaskId: {MyTaskId}",
                    timeTableItemForAdd.UserId, timeTableItemForAdd.MyTaskId);

                return new AddTimeTableItemAnswerStatusDatabase { Status = AddTimeTableItemAnswerStatusDatabase.Bad };
            }
        }

        public async Task<GetByIdAnswerAnswerData> Get(int userId)
        {
            _logger.LogInformation("Начало получения элементов расписания по UserId: {UserId}", userId);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT * FROM timetable_items WHERE user_id = @userId";
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("userId", userId);

                using var reader = await command.ExecuteReaderAsync();
                var timeTableItems = new List<TimeTableItemDatabase>();

                while (await reader.ReadAsync())
                {
                    timeTableItems.Add(new TimeTableItemDatabase(
                        reader.GetInt32("my_task_id"),
                        reader.GetInt32("user_id"),
                        reader.GetInt32("count_from"),
                        reader.GetString("name"),
                        reader.GetInt32("priority"),
                        reader.GetDateTime("start_date_time"),
                        reader.GetDateTime("end_date_time"),
                        reader.GetBoolean("is_complete"),
                        reader.IsDBNull("complete_date_time") ? null : reader.GetDateTime("complete_date_time")
                    ));
                }

                return new GetByIdAnswerAnswerData(
                    new ClassicAnswerStatus { Status = ClassicAnswerStatus.Good },
                    timeTableItems
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении элемента расписания");
                return new GetByIdAnswerAnswerData(
                    new ClassicAnswerStatus { Status = ClassicAnswerStatus.Bad },
                    new List<TimeTableItemDatabase>()
                );
            }
        }

        public async Task<DeleteTaskFromTimeTableAnswerStatusDatabase> Delete(int taskId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "DELETE FROM timetable_items WHERE my_task_id = @taskId";
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("taskId", taskId);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new DeleteTaskFromTimeTableAnswerStatusDatabase { Status = DeleteTaskFromTimeTableAnswerStatusDatabase.TaskIsNotExist };
                }

                return new DeleteTaskFromTimeTableAnswerStatusDatabase { Status = DeleteTaskFromTimeTableAnswerStatusDatabase.Good };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении элемента расписания");
                return new DeleteTaskFromTimeTableAnswerStatusDatabase { Status = DeleteTaskFromTimeTableAnswerStatusDatabase.Bad };
            }
        }

        public async Task<SetCompleteTimeTableItemAnswerStatusDatabase> SetComplete(int taskId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                // Сначала получаем текущее значение is_complete
                var selectSql = @"SELECT is_complete FROM timetable_items WHERE my_task_id = @taskId";

                using var selectCommand = new NpgsqlCommand(selectSql, connection);
                selectCommand.Parameters.AddWithValue("taskId", taskId);

                var currentIsComplete = await selectCommand.ExecuteScalarAsync();

                if (currentIsComplete == null)
                {
                    return new SetCompleteTimeTableItemAnswerStatusDatabase { Status = SetCompleteTimeTableItemAnswerStatusDatabase.TimeTableItemNotExist };
                }
                bool isComplete = (bool)currentIsComplete;
                bool newIsComplete = !isComplete;

                // Обновляем на инвертированное значение
                var updateSql = @"
                        UPDATE timetable_items SET 
                            is_complete = @newIsComplete, 
                            complete_date_time = @completeDateTime 
                        WHERE my_task_id = @taskId";

                using var updateCommand = new NpgsqlCommand(updateSql, connection);
                updateCommand.Parameters.AddWithValue("taskId", taskId);
                updateCommand.Parameters.AddWithValue("newIsComplete", newIsComplete);
                updateCommand.Parameters.AddWithValue("completeDateTime", newIsComplete ? DateTime.Now : (object)DBNull.Value);

                var rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new SetCompleteTimeTableItemAnswerStatusDatabase { Status = SetCompleteTimeTableItemAnswerStatusDatabase.TimeTableItemNotExist };
                }

                return new SetCompleteTimeTableItemAnswerStatusDatabase { Status = SetCompleteTimeTableItemAnswerStatusDatabase.Good };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при пометке элемента расписания");
                return new SetCompleteTimeTableItemAnswerStatusDatabase { Status = SetCompleteTimeTableItemAnswerStatusDatabase.Bad };
            }
        }

        public async Task<SetCompleteForRepitAnswerStatusDatabase> SetCompleteForRepit(int taskId, int countFrom)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var selectSql = @"SELECT is_complete FROM timetable_items WHERE my_task_id = @taskId AND count_from = @countFrom";

                using var selectCommand = new NpgsqlCommand(selectSql, connection);
                selectCommand.Parameters.AddWithValue("taskId", taskId);
                selectCommand.Parameters.AddWithValue("countFrom", countFrom);

                var currentIsComplete = await selectCommand.ExecuteScalarAsync();

                if (currentIsComplete == null)
                {
                    return new SetCompleteForRepitAnswerStatusDatabase { Status = SetCompleteForRepitAnswerStatusDatabase.ItemNotExist };
                }

                bool isComplete = (bool)currentIsComplete;
                bool newIsComplete = !isComplete;

                // Обновляем на инвертированное значение
                var updateSql = @"
                    UPDATE timetable_items SET 
                        is_complete = @newIsComplete, 
                        complete_date_time = @completeDateTime 
                    WHERE my_task_id = @taskId AND count_from = @countFrom";

                using var updateCommand = new NpgsqlCommand(updateSql, connection);
                updateCommand.Parameters.AddWithValue("taskId", taskId);
                updateCommand.Parameters.AddWithValue("countFrom", countFrom);
                updateCommand.Parameters.AddWithValue("newIsComplete", newIsComplete);
                updateCommand.Parameters.AddWithValue("completeDateTime", newIsComplete ? DateTime.Now : (object)DBNull.Value);

                var rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new SetCompleteForRepitAnswerStatusDatabase { Status = SetCompleteForRepitAnswerStatusDatabase.ItemNotExist };
                }

                return new SetCompleteForRepitAnswerStatusDatabase { Status = SetCompleteForRepitAnswerStatusDatabase.Good };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при пометке элемента расписания");
                return new SetCompleteForRepitAnswerStatusDatabase { Status = SetCompleteForRepitAnswerStatusDatabase.Bad };
            }
        }
    }
}
