/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: CustomerOrderViewModel.cs
 * @since: 14/11/2025
 * @function: View model used to present a customer's order history on the dashboard.
 *            Stores the order identifier, order date, and calculated total amount.
 */

namespace ABCRetailWebApp.Models
{
    public class CustomerOrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
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