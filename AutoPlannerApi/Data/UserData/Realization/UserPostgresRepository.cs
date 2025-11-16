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

        public async Task Registrate(UserForRegistrationDatabase userForRegistration)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand(
                    "INSERT INTO users (nickname, password) VALUES (@nickname, @password)",
                    connection);
                command.Parameters.AddWithValue("nickname", userForRegistration.Nickname);
                command.Parameters.AddWithValue("password", userForRegistration.Password);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
