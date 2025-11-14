/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: AdminDashboardViewModel.cs
 * @since: 14/11/2025
 * @function: View model providing summary metrics for the Admin Dashboard,
 *            including product, customer, and order counts retrieved from SQL.
 */

namespace ABCRetailWebApp.Models;

public class AdminDashboardViewModel
{
    public int ProductCount { get; set; }
    public int CustomerCount { get; set; }
    public int OrderCount { get; set; }
}
/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 14 November 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */
