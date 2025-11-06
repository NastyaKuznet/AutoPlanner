using AutoPlannerApi.Data.Common.Model;
using AutoPlannerApi.Data.TimeTableData.Interface;
using AutoPlannerApi.Data.TimeTableData.Model;
using AutoPlannerApi.Data.TimeTableData.Model.Answer;
using AutoPlannerApi.Data.TimeTableData.Model.Answer.AnswerStatus;
using Npgsql;
using System.Data;

namespace AutoPlannerApi.Data.TimeTableData.Realization
{
    public class TimeTableItemPostgresRepository : ITimeTableItemDatabaseRepository
    {
        private readonly string _connectionString;

        public TimeTableItemPostgresRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<AddTimeTableItemAnswerStatusDatabase> Add(TimeTableItemForAddDatabase timeTableItemForAdd)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    INSERT INTO timetable_items (
                        my_task_id, user_id, count_from, name, start_date_time, 
                        end_date_time, is_complete, complete_date_time
                    ) VALUES (
                        @myTaskId, @userId, @countFrom, @name, @startDateTime,
                        @endDateTime, @isComplete, @completeDateTime
                    )";

                using var command = new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("myTaskId", timeTableItemForAdd.MyTaskId);
                command.Parameters.AddWithValue("userId", timeTableItemForAdd.UserId);
                command.Parameters.AddWithValue("countFrom", timeTableItemForAdd.CountFrom);
                command.Parameters.AddWithValue("name", timeTableItemForAdd.Name);
                command.Parameters.AddWithValue("startDateTime", timeTableItemForAdd.StartDateTime);
                command.Parameters.AddWithValue("endDateTime", timeTableItemForAdd.EndDateTime);
                command.Parameters.AddWithValue("isComplete", timeTableItemForAdd.IsComplete);
                command.Parameters.AddWithValue("completeDateTime", timeTableItemForAdd.CompleteDateTime ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();

                return new AddTimeTableItemAnswerStatusDatabase { Status = AddTimeTableItemAnswerStatusDatabase.Good };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding timetable item: {ex.Message}");
                return new AddTimeTableItemAnswerStatusDatabase { Status = AddTimeTableItemAnswerStatusDatabase.Bad };
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
                Console.WriteLine($"Error deleting timetable item: {ex.Message}");
                return new DeleteTaskFromTimeTableAnswerStatusDatabase { Status = DeleteTaskFromTimeTableAnswerStatusDatabase.Bad };
            }
        }

        public async Task<GetByIdAnswerAnswerData> Get(int userId)
        {
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
                Console.WriteLine($"Error getting timetable items: {ex.Message}");
                return new GetByIdAnswerAnswerData(
                    new ClassicAnswerStatus { Status = ClassicAnswerStatus.Bad },
                    new List<TimeTableItemDatabase>()
                );
            }
        }

        public async Task<SetCompleteTimeTableItemAnswerStatusDatabase> SetComplete(int taskId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    UPDATE timetable_items SET 
                        is_complete = true, 
                        complete_date_time = @completeDateTime 
                    WHERE my_task_id = @taskId";

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("taskId", taskId);
                command.Parameters.AddWithValue("completeDateTime", DateTime.Now);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new SetCompleteTimeTableItemAnswerStatusDatabase { Status = SetCompleteTimeTableItemAnswerStatusDatabase.TimeTableItemNotExist };
                }

                return new SetCompleteTimeTableItemAnswerStatusDatabase { Status = SetCompleteTimeTableItemAnswerStatusDatabase.Good };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting timetable item complete: {ex.Message}");
                return new SetCompleteTimeTableItemAnswerStatusDatabase { Status = SetCompleteTimeTableItemAnswerStatusDatabase.Bad };
            }
        }
    }
}
