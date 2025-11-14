/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: HomeController.cs
 * @since [Updated: 14/10/2025]
 * @function: Controller for managing home page 
   and file operations in the ABC Retail Web Application.
 */

using Microsoft.AspNetCore.Mvc;
using ABCRetailWebApp.Services;

namespace ABCRetailWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly SqlAuthService _auth;

        public LoginController(SqlAuthService auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both username and password.";
                return View();
            }

            var result = _auth.AuthenticateUser(username, password);

            if (!result.Success)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            HttpContext.Session.SetString("Username", result.Username!);
            HttpContext.Session.SetString("UserRole", result.Role!);


            if (result.Role == "Customer")
            {
                if (result.CustomerId == null)
                {
                    ViewBag.Error = "Customer ID missing. Contact support.";
                    return View();
                }

                HttpContext.Session.SetString("CustomerId", result.CustomerId!.ToString());
            }

            return result.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Customer" => RedirectToAction("Shop", "Customer"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 14 October 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */