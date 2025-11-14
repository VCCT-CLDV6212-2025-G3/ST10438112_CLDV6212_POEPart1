/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: SqlAuthService.cs
 * @since: 14/11/2025
 * @function: Provides authentication logic for the ABCRetailWebApp by validating user
 *            credentials against the Azure SQL Database. Determines whether the user
 *            is an Admin or Customer, retrieves the associated CustomerId when applicable,
 *            and returns a structured authentication result for session handling.
 */

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ABCRetailWebApp.Services
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Role { get; set; }
        public string? Username { get; set; }
        public int? CustomerId { get; set; }
    }

    public class SqlAuthService
    {
        private readonly string _connection;

        public SqlAuthService(IConfiguration config)
        {
            _connection = config.GetConnectionString("AzureSQLConnection");
        }

        public AuthResult AuthenticateUser(string username, string password)
        {
            using var conn = new SqlConnection(_connection);
            conn.Open();

            string userQuery = @"
                SELECT UserId, Role
                FROM Users
                WHERE Username=@u AND PasswordHash=@p";

            int? userId = null;
            string? role = null;

            using (var cmd = new SqlCommand(userQuery, conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    userId = reader.GetInt32(0);
                    role = reader.GetString(1);
                }
            }

            if (userId == null)
                return new AuthResult { Success = false };

            if (role == "Admin")
            {
                return new AuthResult
                {
                    Success = true,
                    Role = "Admin",
                    Username = username,
                    CustomerId = null
                };
            }

            string custQuery = @"
                SELECT CustomerId
                FROM Customers
                WHERE UserId=@uid";

            int? customerId = null;

            using (var cmd = new SqlCommand(custQuery, conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId.Value);
                var obj = cmd.ExecuteScalar();
                if (obj != null) customerId = Convert.ToInt32(obj);
            }

            return new AuthResult
            {
                Success = true,
                Role = "Customer",
                Username = username,
                CustomerId = customerId
            };
        }
    }
}
/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 14 November 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */