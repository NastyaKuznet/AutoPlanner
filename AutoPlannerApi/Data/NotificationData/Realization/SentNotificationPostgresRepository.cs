using AutoPlannerApi.Data.NotificationData.Interface;
using Npgsql;

namespace AutoPlannerApi.Data.NotificationData.Realization
{
    public class SentNotificationPostgresRepository : ISentNotificationRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SentNotificationPostgresRepository> _logger;

        public SentNotificationPostgresRepository(string connectionString, ILogger<SentNotificationPostgresRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<bool> ExistsAsync(int userId, int taskId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT 1 FROM sent_notifications WHERE user_id = @userId AND task_id = @taskId";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("taskId", taskId);

            var result = await command.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<bool> AddAsync(int userId, int taskId, string jobId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO sent_notifications (user_id, task_id, job_id, created_at)
                VALUES (@userId, @taskId, @jobId, @createdAt)
                ON CONFLICT (user_id, task_id) DO UPDATE SET job_id = @jobId";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("taskId", taskId);
            command.Parameters.AddWithValue("jobId", jobId);
            command.Parameters.AddWithValue("createdAt", DateTime.Now);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                _logger.LogDebug("Добавлена запись: UserId={UserId}, TaskId={TaskId}, JobId={JobId}", userId, taskId, jobId);
            }

            return rowsAffected > 0;
        }

        public async Task<List<string>> RemoveByUserAsync(int userId)
        {
            var jobIds = new List<string>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var selectSql = "SELECT job_id FROM sent_notifications WHERE user_id = @userId";
            using var selectCommand = new NpgsqlCommand(selectSql, connection);
            selectCommand.Parameters.AddWithValue("userId", userId);

            using var reader = await selectCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (!reader.IsDBNull(0))
                {
                    jobIds.Add(reader.GetString(0));
                }
            }

            await reader.CloseAsync();
                
            var deleteSql = "DELETE FROM sent_notifications WHERE user_id = @userId";
            using var deleteCommand = new NpgsqlCommand(deleteSql, connection);
            deleteCommand.Parameters.AddWithValue("userId", userId);

            var rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Удалены все уведомления пользователя {UserId}, JobIds: {JobIdsCount}", userId, jobIds.Count);
            }

            return jobIds;
        }
    }
}
