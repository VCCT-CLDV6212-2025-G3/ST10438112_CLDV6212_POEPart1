/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: OrdersController.cs
 * @since: 15/11/2025
 * @function: Handles customer order history and order details.
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ABCRetailWebApp.Models;

namespace ABCRetailWebApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IConfiguration _config;

        public OrdersController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult MyOrders()
        {
            if (HttpContext.Session.GetString("Role") != "Customer")
                return RedirectToAction("Index", "Login");

            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            var orders = new List<CustomerOrderViewModel>();

            using (var conn = new SqlConnection(_config.GetConnectionString("AzureSQLConnection")))
            {
                conn.Open();

                string sql = @"
                    SELECT OrderId, OrderDate, TotalAmount
                    FROM Orders
                    WHERE CustomerId=@cid
                    ORDER BY OrderDate DESC";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cid", customerId);

                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    orders.Add(new CustomerOrderViewModel
                    {
                        OrderId = rdr.GetInt32(0),
                        OrderDate = rdr.GetDateTime(1),
                        TotalAmount = rdr.GetDecimal(2)
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
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 15 November 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */