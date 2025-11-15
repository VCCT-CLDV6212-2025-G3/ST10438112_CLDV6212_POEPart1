/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: Program.cs
 * @since: [Updated: 14/11/2025]
 * @function: This is the entry point for the ABCRetailWebApp application.
 */

//-----------namespace ABCRetailWebApp------------------//
using ABCRetailWebApp.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var storageCs = builder.Configuration.GetSection("Storage")["ConnectionString"];
if (string.IsNullOrWhiteSpace(storageCs))
    Console.WriteLine("WARNING: Missing Storage:ConnectionString");

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("AzureSQLConnection"))
    && string.IsNullOrWhiteSpace(builder.Configuration["AzureSQLConnection"]))
{
    Console.WriteLine("WARNING: Missing AzureSQLConnection");
}

builder.Services.AddScoped<SqlAuthService>();

builder.Services.AddSingleton(new BlobStorageService(storageCs));
builder.Services.AddSingleton(new QueueStorageService(storageCs));
builder.Services.AddSingleton(new FileStorageService(storageCs));
builder.Services.AddSingleton(new TableStorageService(storageCs));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
//-----------------------------------------------------------------//

// END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 14 November 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */