/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: Order.cs
 * @since [Updated: 14/11/2025]
 * @function: Represents a customer order before SQL persistence is implemented.
 */

namespace ABCRetailWebApp.Models
{
    public class Order
    {
        public int OrderId { get; set; } 
        public string CustomerId { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public double TotalAmount { get; set; }
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