/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: Program.cs
 * @since: [Updated: 22/08/2025]
 * @function: This is the entry point for the ABCRetailWebApp application.
 */

//-----------namespace ABCRetailWebApp------------------//
using ABCRetailWebApp.Services;

//-----------------using directives---------------------//
//this section includes necessary namespaces for the application
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
//---------------------------------------------------//

//------------------Azure Storage Connection String------------------//
//this section retrieves the Azure Storage connection string from configuration
var storageConn = builder.Configuration["AzureStorageConnectionString"];
if (string.IsNullOrWhiteSpace(storageConn))
    throw new InvalidOperationException("AzureStorageConnectionString is missing.");
//-------------------------------------------------------------------//

//------------------Register Storage Services------------------------//
// Register your storage services
builder.Services.AddSingleton(new BlobStorageService(storageConn));
builder.Services.AddSingleton(new FileStorageService(storageConn));
builder.Services.AddSingleton(new QueueStorageService(storageConn));
builder.Services.AddSingleton(new TableStorageService(storageConn));
//-------------------------------------------------------------------//

//------------------Configure Application----------------------------//
//this section configures the HTTP request pipeline and routing
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

app.Run();
//------------------------------------------------------------------//

//-----------------------------------------------------------------//
// END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 22 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */