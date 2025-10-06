/*
@author: Kylan Chart Frittelli ST10438112
@file: ProcessTransaction.cs
@since [Updated: 06/10/25]
@function: consumes 'orders' queue messages and saves each as a json file under 'contracts'
*/

//----------------------using directives----------------------------//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
//------------------------------------------------------------------//

namespace ABCRetailFunctions
{
    //----------------------ProcessTransaction class----------------------------//
    public class ProcessTransaction
    {
        private readonly ILogger<ProcessTransaction> _log;
        private readonly string _connString;

        //----------------------constructor----------------------------//
        public ProcessTransaction(ILogger<ProcessTransaction> log, IConfiguration config)
        {
            _log = log;
            _connString = config["AzureWebJobsStorage"]
                          ?? throw new InvalidOperationException("AzureWebJobsStorage missing.");
        }
        //--------------------------------------------------------------//

        //----------------------RunAsync method----------------------------//
        [Function("ProcessTransaction")]
        public async Task RunAsync(
            [QueueTrigger("orders", Connection = "AzureWebJobsStorage")] string queueMessage,
            FunctionContext context)
        {
            //guard for empty message
            if (string.IsNullOrWhiteSpace(queueMessage))
            {
                _log.LogWarning("Received empty queue message; aborting.");
                return;
            }

            //try to parse json into dto
            TransactionDto? dto = null;
            try
            {
                dto = JsonSerializer.Deserialize<TransactionDto>(queueMessage);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Message is not valid JSON; will write raw text instead.");
            }

            //build filename
            var idPart = dto?.Id?.ToString() ?? Guid.NewGuid().ToString("N");
            var fileName = $"order-{idPart}-{DateTime.UtcNow:yyyyMMdd-HHmmssfff}.json";

            //prepare content
            const string shareName = "contracts";
            var contentBytes = Encoding.UTF8.GetBytes(queueMessage);

            try
            {
                //----------------------fileshare operations----------------------------//
                var share = new ShareClient(_connString, shareName);
                _log.LogInformation("File share URI: {Uri}", share.Uri);

                await share.CreateIfNotExistsAsync();

                var rootDir = share.GetRootDirectoryClient();

                var file = rootDir.GetFileClient(fileName);

                await file.CreateAsync(contentBytes.Length);
                using var ms = new MemoryStream(contentBytes, writable: false);
                await file.UploadRangeAsync(new Azure.HttpRange(0, contentBytes.Length), ms);

                _log.LogInformation("Wrote contract file: {FileUri}", file.Uri);
                //--------------------------------------------------------------//
            }
            catch (RequestFailedException ex)
            {
                _log.LogError(ex, "Azure Files write failed {Status}/{Code} for {FileName}", ex.Status, ex.ErrorCode, fileName);
                throw;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Unexpected failure writing {FileName}", fileName);
                throw;
            }
        }
        //--------------------------------------------------------------//
    }
    //------------------------------------------------------------------//

    //----------------------TransactionDto class----------------------------//
    public sealed class TransactionDto
    {
        public Guid? Id { get; set; }
        public string? CustomerName { get; set; }
        public decimal? Total { get; set; }
    }
    //--------------------------------------------------------------//
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