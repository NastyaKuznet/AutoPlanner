using AutoPlannerApi.Data.UserData.Interface;
using AutoPlannerApi.Data.UserData.Model;
using AutoPlannerApi.Data.UserData.Model.AnswerStatus;
using Npgsql;
using System.Data;

namespace AutoPlannerApi.Data.UserData.Realization
{
    public class UserPostgresRepository : IUserDatabaseRepository
    {
        private readonly string _connectionString;

        public UserPostgresRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CheckAnswerStatusDatabase> Check(int userId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand("SELECT id FROM users WHERE id = @id", connection);
                command.Parameters.AddWithValue("id", userId);
                var result = await command.ExecuteScalarAsync();
                if (result != null)
                {
                    return new CheckAnswerStatusDatabase { Status = CheckAnswerStatusDatabase.UserExist };
                }
                else
                {
                    return new CheckAnswerStatusDatabase { Status = CheckAnswerStatusDatabase.UserNotExist };
                }
            }
        }

        public async Task<IReadOnlyCollection<UserDatabase>> GetUsers()
        {
            var users = new List<UserDatabase>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand("SELECT id, nickname, password FROM users", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new UserDatabase(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2)));
                    }
                }
            }
            return users;
        }

        public async Task<int> Registrate(UserForRegistrationAndAuthorizationDatabase userForRegistration)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand(
                    "INSERT INTO users (nickname, password) VALUES (@nickname, @password) RETURNING id",
                    connection);
                command.Parameters.AddWithValue("nickname", userForRegistration.Nickname);
                command.Parameters.AddWithValue("password", userForRegistration.Password);
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        public async Task<UserDatabase> GetUserByTelegramChatId(long chatId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand(
                    "SELECT id, nickname, password, telegram_chat_id FROM users WHERE telegram_chat_id = @chatId",
                    connection);
                command.Parameters.AddWithValue("chatId", chatId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new UserDatabase(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.IsDBNull(3) ? null : reader.GetInt64(3));
                    }
                }
            }
            return null;
        }

        public async Task<UserDatabase> GetUserById(int userId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand(
                    "SELECT id, nickname, password, telegram_chat_id FROM users WHERE id = @id",
                    connection);
                command.Parameters.AddWithValue("id", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new UserDatabase(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.IsDBNull(3) ? null : reader.GetInt64(3));
                    }
                }
            }
            return null;
        }

        public async Task<bool> UpdateUserTelegramChatId(int userId, long chatId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand(
                    "UPDATE users SET telegram_chat_id = @chatId WHERE id = @userId",
                    connection);
                command.Parameters.AddWithValue("chatId", chatId);
                command.Parameters.AddWithValue("userId", userId);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
    }
}
