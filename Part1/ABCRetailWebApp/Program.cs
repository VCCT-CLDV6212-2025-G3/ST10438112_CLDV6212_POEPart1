/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: Program.cs
 * @since: [Updated: 05/08/2025]
 * @function: This is the entry point for the ABCRetailWebApp application.
 */

//-----------namespace ABCRetailWebApp------------------//
using ABCRetailWebApp.Services;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var storageCs = builder.Configuration["Storage:ConnectionString"];
if (string.IsNullOrWhiteSpace(storageCs))
    throw new InvalidOperationException("Missing Storage:ConnectionString");

builder.Services.AddSingleton(new BlobStorageService(storageCs));
builder.Services.AddSingleton(new QueueStorageService(storageCs));
builder.Services.AddSingleton(new FileStorageService(storageCs));
builder.Services.AddSingleton(new TableStorageService(storageCs));
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureStorageConnectionString:blobServiceUri"]!).WithName("AzureStorageConnectionString");
    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureStorageConnectionString:queueServiceUri"]!).WithName("AzureStorageConnectionString");
    clientBuilder.AddTableServiceClient(builder.Configuration["AzureStorageConnectionString:tableServiceUri"]!).WithName("AzureStorageConnectionString");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/smoke/storage", () => Results.Ok("Storage DI wired."));

app.Run();

//-----------------------------------------------------------------//
// END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 05 October 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */