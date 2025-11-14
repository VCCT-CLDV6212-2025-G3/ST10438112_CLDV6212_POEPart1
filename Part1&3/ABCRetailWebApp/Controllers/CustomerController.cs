/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: CustomerController.cs
 * @since [Updated: 14/11/2025]
 * @function: Manages the customer dashboard by
 * retrieving a customer’s past orders from the Azure SQL database.
 */


using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ABCRetailWebApp.Models;

namespace ABCRetailWebApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly string _connection;

        public CustomerController(IConfiguration config)
        {
            _connection = config.GetConnectionString("AzureSQLConnection");
        }

        public IActionResult Dashboard()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Login");

            var orders = new List<CustomerOrderViewModel>();

            using (var conn = new SqlConnection(_connection))
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        o.OrderId, 
                        o.OrderDate, 
                        SUM(oi.LineTotal) AS TotalAmount
                    FROM Orders o
                    JOIN OrderItems oi ON o.OrderId = oi.OrderId
                    WHERE o.CustomerId = @cid
                    GROUP BY o.OrderId, o.OrderDate
                    ORDER BY o.OrderDate DESC";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cid", int.Parse(customerId));

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new CustomerOrderViewModel
                    {
                        OrderId = reader.GetInt32(0),
                        OrderDate = reader.GetDateTime(1),
                        TotalAmount = reader.GetDecimal(2)
                    });
                }
            }

            return View(orders);
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