/*
@author: Kylan Chart Frittelli ST10438112
@file: Program.cs
@function: Application bootstrap (Functions worker, configuration, DI)
@since [Updated: 06/10/25]
@function: Azure client registrations (Blob/Queue/File/Table)
*/

//----------------------Using directives----------------------------//
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//------------------------------------------------------------------//

//----------------------Bootstrap functions app--------------------//
var builder = FunctionsApplication.CreateBuilder(args);
//-------------------------------------------------------------------//

//----------------------Configuration sources-----------------------//
builder.Configuration
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();
//-----------------------------------------------------------------//

//----------------------Resolve storage connection--------------------//
//(chatgpt was consulted to fix storage connection issues found in my code)
var storageConn =
    builder.Configuration["StorageAccountConnection"]
    ?? builder.Configuration["AzureWebJobsStorage"];

if (string.IsNullOrWhiteSpace(storageConn))
{
    throw new InvalidOperationException(
        "Missing storage connection string. Set 'StorageAccountConnection' (preferred) or 'AzureWebJobsStorage'.");
}
//---------------------------------------------------------------------//

//----------------------Azure client Registerations (DI)-----------------------//
builder.Services.AddSingleton(new BlobServiceClient(storageConn));
builder.Services.AddSingleton(new QueueServiceClient(storageConn));
builder.Services.AddSingleton(new ShareServiceClient(storageConn));
builder.Services.AddSingleton(new TableServiceClient(storageConn));
//----------------------------------------------------------------------------//

//----------------------Build and run host---------------------//
builder.Build().Run();
//-----------------------------------------------------------//

//END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 * Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022. Azure for Developers. 2nd ed. Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022. The Developer’s Guide to Azure. Redmond: Microsoft Press.
 * OpenAI, 2025. ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 06 October 2025].
 * Github Inc., 2025. GitHub Copilot. [online] Available at: https://github.com [Accessed 06 October 2025].
 */