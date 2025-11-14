/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: CartController.cs
 * @since [Updated: 14/11/2025]
 * @functionImplements the shopping cart workflow using session state.:.
 */

using Microsoft.AspNetCore.Mvc;
using ABCRetailWebApp.Models;
using ABCRetailWebApp.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ABCRetailWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly IConfiguration _config;

        public CartController(IConfiguration config)
        {
            _config = config;
        }


        private List<CartItem> GetCart()
        {
            return HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObject("Cart", cart);
        }

        public IActionResult Add(int id, string name, double price)
        {
            var cart = GetCart();
            var existing = cart.FirstOrDefault(x => x.ProductId == id);

            if (existing == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = id,
                    Name = name,
                    Price = price,
                    Quantity = 1
                });
            }
            else
            {
                existing.Quantity++;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            return View(GetCart());
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
                cart.Remove(item);

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cart = GetCart();
            var customerId = HttpContext.Session.GetString("CustomerId");

            if (string.IsNullOrEmpty(customerId))
                return RedirectToAction("Index", "Login");

            if (cart.Count == 0)
                return RedirectToAction("Index");

            return View(cart);
        }

        [HttpPost]
        public IActionResult PlaceOrder()
        {
            var cart = GetCart();
            var customerIdString = HttpContext.Session.GetString("CustomerId");

            if (string.IsNullOrEmpty(customerIdString))
                return RedirectToAction("Index", "Login");

            if (cart.Count == 0)
                return RedirectToAction("Index");

            int customerId = int.Parse(customerIdString);

            var cs = _config.GetConnectionString("AzureSQLConnection");
            using var conn = new SqlConnection(cs);
            conn.Open();

            string insertOrder = @"
                INSERT INTO Orders (CustomerId, TotalAmount)
                OUTPUT INSERTED.OrderId
                VALUES (@custId, @total)";

            double total = cart.Sum(c => c.LineTotal);

            using var cmdOrder = new SqlCommand(insertOrder, conn);
            cmdOrder.Parameters.AddWithValue("@custId", customerId);
            cmdOrder.Parameters.AddWithValue("@total", total);

            int orderId = (int)cmdOrder.ExecuteScalar();

            string insertItem = @"
                INSERT INTO OrderItems (OrderId, ProductId, Quantity, LineTotal)
                VALUES (@oId, @pId, @qty, @line)";

            foreach (var item in cart)
            {
                using var cmdItem = new SqlCommand(insertItem, conn);
                cmdItem.Parameters.AddWithValue("@oId", orderId);
                cmdItem.Parameters.AddWithValue("@pId", item.ProductId);
                cmdItem.Parameters.AddWithValue("@qty", item.Quantity);
                cmdItem.Parameters.AddWithValue("@line", item.LineTotal);
                cmdItem.ExecuteNonQuery();
            }

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("OrderSuccess", new { id = orderId });
        }

        public IActionResult OrderSuccess(int id)
        {
            ViewBag.OrderId = id;
            return View();
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