/*
@author: Kylan Chart Frittelli ST10438112
@file: AddProductToTable.cs
@since [Updated: 06/10/25]
@function: validates json payload and stores it inside the 'Products' table
*/

//----------------------using directives----------------------------//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Azure.Data.Tables;
using ABCRetail.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Azure;
using Microsoft.Extensions.Logging;
//------------------------------------------------------------------//

namespace ABCRetail.Functions.Functions
{
    //----------------------AddProductToTable class----------------------------//
    public class AddProductToTable(ILogger<AddProductToTable> log)
    {
        //----------------------Run method----------------------------//
        [Function("AddProductToTable")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequestData req)
        {
            //read incoming json from request
            var res = req.CreateResponse();
            string json = await new StreamReader(req.Body, Encoding.UTF8).ReadToEndAsync();
            log.LogInformation("Incoming JSON: {json}", json);

            //attempt to deserialize payload into ProductEntity
            ProductEntity dto;
            try
            {
                dto = JsonSerializer.Deserialize<ProductEntity>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new();
            }
            catch (Exception ex)
            {
                res.StatusCode = System.Net.HttpStatusCode.BadRequest;
                await res.WriteStringAsync($"Bad JSON: {ex.Message}");
                return res;
            }
            //------------------------------------------------------------//

            //----------------------Payload validation----------------------------//
            dto.PartitionKey ??= "Products";
            dto.RowKey = string.IsNullOrWhiteSpace(dto.RowKey) ? Guid.NewGuid().ToString("N") : dto.RowKey;

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                res.StatusCode = System.Net.HttpStatusCode.BadRequest;
                await res.WriteStringAsync("Invalid payload. Expecting: { name, description, price }");
                return res;
            }
            //------------------------------------------------------------//

            //----------------------Retrieve connection string----------------------------//
            var cs = Environment.GetEnvironmentVariable("StorageAccountConnection");
            if (string.IsNullOrWhiteSpace(cs))
            {
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                await res.WriteStringAsync("Config error: StorageAccountConnection is missing.");
                return res;
            }
            //------------------------------------------------------------//

            //----------------------Table operations----------------------------//
            var table = new TableClient(cs, "Products");

            try
            {
                await table.CreateIfNotExistsAsync();
                await table.AddEntityAsync(dto);
            }
            catch (RequestFailedException rex)
            {
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                await res.WriteStringAsync($"Insert failed (Storage): {rex.Status} {rex.ErrorCode} - {rex.Message}");
                return res;
            }
            catch (Exception ex)
            {
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                await res.WriteStringAsync($"Insert failed: {ex.Message}");
                return res;
            }
            //------------------------------------------------------------//

            //----------------------Return success response----------------------------//
            await res.WriteAsJsonAsync(new { message = "Product added.", id = dto.RowKey });
            return res;
            //------------------------------------------------------------//
        }
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