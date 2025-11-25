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

        public async Task<bool> AddAsync(int userId, int taskId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO sent_notifications (user_id, task_id, created_at)
                VALUES (@userId, @taskId, @createdAt)
                ON CONFLICT (user_id, task_id) DO NOTHING";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("taskId", taskId);
            command.Parameters.AddWithValue("createdAt", DateTime.Now);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                _logger.LogDebug("Добавлена запись: UserId={UserId}, TaskId={TaskId}", userId, taskId);
            }

            return rowsAffected > 0;
        }

        public async Task<bool> RemoveAsync(int userId, int taskId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM sent_notifications WHERE user_id = @userId AND task_id = @taskId";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("taskId", taskId);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                _logger.LogDebug("Удалена запись: UserId={UserId}, TaskId={TaskId}", userId, taskId);
            }

            return rowsAffected > 0;
        }

        public async Task<bool> RemoveByUserAsync(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM sent_notifications WHERE user_id = @userId";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("userId", userId);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Удалены все уведомления пользователя {UserId}", userId);
            }

            return rowsAffected > 0;
        }

        public async Task<bool> RemoveByTaskAsync(int taskId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM sent_notifications WHERE task_id = @taskId";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("taskId", taskId);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Удалены все уведомления для задачи {TaskId}", taskId);
            }

            return rowsAffected > 0;
        }
    }
}
