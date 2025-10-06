/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: Customer.cs
 * @since [Updated: 22/08/2025]
 * @functin: Model for Customer entity in the ABC Retail Web Application.
 */

//---------------namespace------------------------------------------//
namespace ABCRetailWebApp.Models;
using Azure;
using Azure.Data.Tables;

//----------------Customer Model-------------------------------//
public class Customer : ITableEntity
{
    //properties for the Customer entity
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }        
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
//-------------------------------------------------------------//

//------------------------------------------------------------//
//END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 22 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */
