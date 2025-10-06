/*
@author: Kylan Chart Frittelli ST10438112
@file: EnqueueTransaction.cs
@since [Updated: 06/10/25]
@function: validates json payload and posts message to 'orders' queue in base64 format
*/

//----------------------using directives----------------------------//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
//------------------------------------------------------------------//

namespace ABCRetail.Functions
{
    //----------------------EnqueueTransaction class----------------------------//
    public class EnqueueTransaction
    {
        private readonly ILogger<EnqueueTransaction> _logger;
        private readonly string _storageConnection;
        private const string QueueName = "orders";

        //----------------------constructor----------------------------//
        public EnqueueTransaction(ILogger<EnqueueTransaction> logger)
        {
            _logger = logger;
            _storageConnection = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                                ?? throw new InvalidOperationException("AzureWebJobsStorage is not configured.");
        }
        //--------------------------------------------------------------//

        //----------------------Run method----------------------------//
        [Function("EnqueueTransaction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "transactions/enqueue")]
            HttpRequestData req)
        {
            try
            {
                //read body from request
                using var body = new StreamReader(req.Body);
                var json = await body.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(json))
                    return await BadRequest(req, "Empty request body.");

                //----------------------deserialize transactiondto----------------------------//
                TransactionDto? dto;
                try
                {
                    dto = JsonSerializer.Deserialize<TransactionDto>(json, JsonOpts);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Invalid JSON payload");
                    return await BadRequest(req, "Invalid JSON.");
                }

                if (dto is null)
                    return await BadRequest(req, "Unable to parse JSON.");
                //--------------------------------------------------------------//

                //----------------------validate data----------------------------//
                var errors = Validate(dto);
                if (errors.Count > 0)
                    return await BadRequest(req, $"Validation failed: {string.Join("; ", errors)}");
                //--------------------------------------------------------------//

                //----------------------prepare transaction details----------------------------//
                dto.CorrelationId ??= Guid.NewGuid().ToString();
                dto.EnqueuedAtUtc = DateTime.UtcNow;

                var messageJson = JsonSerializer.Serialize(dto, JsonOpts);
                var byteCount = Encoding.UTF8.GetByteCount(messageJson);
                if (byteCount > 60 * 1024)
                    return await BadRequest(req, $"Message too large ({byteCount} bytes). Keep under ~60KB.");
                //--------------------------------------------------------------//

                //----------------------send message to queue----------------------------//
                var qOpts = new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 };
                var queue = new QueueClient(_storageConnection, QueueName, qOpts);
                await queue.CreateIfNotExistsAsync();
                var sendResult = await queue.SendMessageAsync(messageJson);

                _logger.LogInformation("Enqueued message for OrderId={OrderId}, CorrelationId={CorrelationId}",
                    dto.OrderId, dto.CorrelationId);

                var ok = req.CreateResponse(HttpStatusCode.Accepted);
                await ok.WriteAsJsonAsync(new
                {
                    status = "enqueued",
                    queue = QueueName,
                    dto.OrderId,
                    dto.CustomerId,
                    dto.CorrelationId,
                    messageId = sendResult?.Value?.MessageId,
                    popReceipt = sendResult?.Value?.PopReceipt
                });
                return ok;
                //--------------------------------------------------------------//
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue transaction");
                var resp = req.CreateResponse(HttpStatusCode.InternalServerError);
                await resp.WriteStringAsync("Internal error while enqueuing the transaction.");
                return resp;
            }
        }
        //--------------------------------------------------------------//

        //----------------------TransactionDto class----------------------------//
        public class TransactionDto
        {
            [JsonPropertyName("orderId")] public string? OrderId { get; set; }
            [JsonPropertyName("customerId")] public string? CustomerId { get; set; }
            [JsonPropertyName("items")] public List<OrderItem>? Items { get; set; }
            [JsonPropertyName("notes")] public string? Notes { get; set; }
            [JsonPropertyName("correlationId")] public string? CorrelationId { get; set; }
            [JsonPropertyName("enqueuedAtUtc")] public DateTime? EnqueuedAtUtc { get; set; }
        }
        //--------------------------------------------------------------//

        //----------------------OrderItem class----------------------------//
        public class OrderItem
        {
            [JsonPropertyName("productId")] public string? ProductId { get; set; }
            [JsonPropertyName("qty")] public int Quantity { get; set; }
            [JsonPropertyName("price")] public double Price { get; set; }
        }
        //--------------------------------------------------------------//

        //----------------------json options----------------------------//
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        //--------------------------------------------------------------//

        //----------------------Validate method----------------------------//
        private static List<string> Validate(TransactionDto dto)
        {
            var issues = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.OrderId)) issues.Add("orderId is required");
            if (string.IsNullOrWhiteSpace(dto.CustomerId)) issues.Add("customerId is required");
            if (dto.Items is null || dto.Items.Count == 0) issues.Add("items must contain at least 1 entry");

            if (dto.Items is not null)
            {
                for (int i = 0; i < dto.Items.Count; i++)
                {
                    var it = dto.Items[i];
                    if (string.IsNullOrWhiteSpace(it.ProductId)) issues.Add($"items[{i}].productId is required");
                    if (it.Quantity <= 0) issues.Add($"items[{i}].qty must be > 0");
                    if (it.Price < 0) issues.Add($"items[{i}].price cannot be negative");
                }
            }
            return issues;
        }
        //--------------------------------------------------------------//

        //----------------------BadRequest helper----------------------------//
        private static async Task<HttpResponseData> BadRequest(HttpRequestData req, string message)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteAsJsonAsync(new { error = message });
            return bad;
        }
        //--------------------------------------------------------------//
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