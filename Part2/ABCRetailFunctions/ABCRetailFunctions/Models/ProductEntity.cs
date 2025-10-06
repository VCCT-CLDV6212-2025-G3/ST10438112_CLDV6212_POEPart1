/*
@author: Kylan Chart Frittelli ST10438112
@file: ProductEntity.cs
@since [Updated: 06/10/25]
@function: implements itableentity for table operations within the 'Products' table
*/

//----------------------using directives----------------------------//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
//------------------------------------------------------------------//

namespace ABCRetail.Functions.Models
{
    //----------------------ProductEntity class----------------------------//
    public sealed class ProductEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "Products";
        public string RowKey { get; set; } = Guid.NewGuid().ToString("N");

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public double Price { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
    //------------------------------------------------------------------//
}

//END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 * Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022. Azure for Developers. 2nd ed. Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022. The Developer’s Guide to Azure. Redmond: Microsoft Press.
 * OpenAI, 2025. ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 06 October 2025].
 * Github Inc., 2025. GitHub Copilot. [online] Available at: https://github.com [Accessed 06 October 2025].
 */