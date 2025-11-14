/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: LoginController.cs
 * @since [Updated: 14/11/2025]
 * @function:Handles administrative data reporting
 * by retrieving aggregated metrics from the Azure SQL Database.
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ABCRetailWebApp.Models;

namespace ABCRetailWebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly string _connection;

        public AdminController(IConfiguration config)
        {
            _connection = config.GetConnectionString("AzureSQLConnection");
        }

        public IActionResult Dashboard()
        {
            var data = new AdminDashboardViewModel();

            using var conn = new SqlConnection(_connection);
            conn.Open();

            data.ProductCount = (int)new SqlCommand("SELECT COUNT(*) FROM Products", conn).ExecuteScalar();
            data.CustomerCount = (int)new SqlCommand("SELECT COUNT(*) FROM Customers", conn).ExecuteScalar();
            data.OrderCount = (int)new SqlCommand("SELECT COUNT(*) FROM Orders", conn).ExecuteScalar();

            return View(data);
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