/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: Cart.cs
 * @since: 14/11/2025
 * @function: Represents the user's shopping cart stored in session state.
 *            CartItem defines product entries with quantity, pricing, and computed line totals.
 */

namespace ABCRetailWebApp.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new();
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public double Price { get; set; }
        public int Quantity { get; set; }

        public double LineTotal => Price * Quantity;
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