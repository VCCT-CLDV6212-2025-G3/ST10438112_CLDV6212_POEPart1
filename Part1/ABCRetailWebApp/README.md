# ABCRetailWebApp_Source
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

/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: Product.cs
 * @since [Updated: 22/08/2025]
 * @function: Model for Product entity in the ABC Retail Web Application.
 */

//---------------namespace------------------------------------------//
namespace ABCRetailWebApp.Models;
using Azure;
using Azure.Data.Tables;

//----------------Product Model-------------------------------//
public class Product : ITableEntity
{
    //properties for the Product entity
    public string PartitionKey { get; set; }  
    public string RowKey { get; set; }        
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
//-------------------------------------------------------------//

//-------------------------------------------------------------//
// END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>//

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 22 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */

/*
 * @authro: Kylan Chart Frittelli (ST10438112)
 * @file: BlobStorageService.cs
 * @since [Updated: 22/08/2025]
 * @function: Service for managing file operations in Azure Blob Storage.
 */

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;

//---------------namespace------------------------------------------//
namespace ABCRetailWebApp.Services
{
    //-------------------BlobStorageService-------------------------------//
    //this service manages file operations in Azure Blob Storage
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        //----------------Constructor-------------------------------//
        //this constructor initializes the BlobServiceClient with the connection string
        public BlobStorageService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }
        //-----------------------------------------------------------//

        //--------------------File Operations-------------------------------//
        //these methods handle file upload and download operations
        public async Task UploadFileAsync(string containerName, string fileName, Stream fileStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        // Download a file from a container
        public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }
        //-----------------------------------------------------------//

        //--------------------List and Delete Operations-------------------------------//
        //these methods handle listing and deleting blobs in a container
        // List all blobs in a container
        public async Task<List<string>> ListFilesAsync(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = containerClient.GetBlobsAsync();
            var result = new List<string>();
            await foreach (BlobItem blob in blobs)
            {
                result.Add(blob.Name);
            }
            return result;
        }
        // Delete a blob from a container
        public async Task DeleteFileAsync(string containerName, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
        //-----------------------------------------------------------//
    }
    //-----------------------------------------------------------//
}
//------------------------------------------------------------//
// END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 22 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */

/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: FileStorageService.cs
 * @since [Updated: 22/08/2025]
 * @function: Service for managing file operations in Azure File Storage
 */

using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System.IO;
using System.Threading.Tasks;

//---------------namespace------------------------------------------//
namespace ABCRetailWebApp.Services
{
    //-------------------FileStorageService-------------------------------//
    //this service manages file operations in Azure File Storage
    public class FileStorageService
    {
        private readonly ShareServiceClient _fileServiceClient;

        //----------------Constructor-------------------------------//
        //this constructor initializes the ShareServiceClient with the connection string
        public FileStorageService(string connectionString)
        {
            _fileServiceClient = new ShareServiceClient(connectionString);
        }
        //-----------------------------------------------------------//

        //--------------------File Operations-------------------------------//
        //these methods handle file upload, download, listing, and deletion operations
        // Upload a file to a share and directory
        public async Task UploadFileAsync(string shareName, string directoryName, string fileName, Stream fileStream)
        {
            var shareClient = _fileServiceClient.GetShareClient(shareName);
            await shareClient.CreateIfNotExistsAsync();
            var dirClient = shareClient.GetDirectoryClient(directoryName);
            await dirClient.CreateIfNotExistsAsync();
            var fileClient = dirClient.GetFileClient(fileName);
            await fileClient.CreateAsync(fileStream.Length);
            await fileClient.UploadRangeAsync(
                new HttpRange(0, fileStream.Length),
                fileStream);
        }

        // Download a file from a share and directory
        public async Task<Stream> DownloadFileAsync(string shareName, string directoryName, string fileName)
        {
            var shareClient = _fileServiceClient.GetShareClient(shareName);
            var dirClient = shareClient.GetDirectoryClient(directoryName);
            var fileClient = dirClient.GetFileClient(fileName);
            var download = await fileClient.DownloadAsync();
            return download.Value.Content;
        }

        // List all files in a directory
        public async Task<List<string>> ListFilesAsync(string shareName, string directoryName)
        {
            var shareClient = _fileServiceClient.GetShareClient(shareName);
            var dirClient = shareClient.GetDirectoryClient(directoryName);
            var files = dirClient.GetFilesAndDirectoriesAsync();
            var result = new List<string>();
            await foreach (var item in files)
            {
                if (!item.IsDirectory)
                    result.Add(item.Name);
            }
            return result;
        }

        // Delete a file from a share and directory
        public async Task DeleteFileAsync(string shareName, string directoryName, string fileName)
        {
            var shareClient = _fileServiceClient.GetShareClient(shareName);
            var dirClient = shareClient.GetDirectoryClient(directoryName);
            var fileClient = dirClient.GetFileClient(fileName);
            await fileClient.DeleteIfExistsAsync();
        }
        //-----------------------------------------------------------//
    }
    //-----------------------------------------------------------//
}
//-----------------------------------------------------------//
//END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 22 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */

/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: QueueStorageService.cs
 * @since [Updated: 22/08/2025]
 * @function: Service for managing queue operations in Azure Queue Storage
 */

using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

using System.Threading.Tasks;

//---------------namespace------------------------------------------//
namespace ABCRetailWebApp.Services
{
    //-------------------QueueStorageService-------------------------------//
    //this service manages queue operations in Azure Queue Storage
    public class QueueStorageService
    {
        private readonly QueueServiceClient _queueServiceClient;

        //----------------Constructor-------------------------------//
        //this constructor initializes the QueueServiceClient with the connection string
        public QueueStorageService(string connectionString)
        {
            _queueServiceClient = new QueueServiceClient(connectionString);
        }
        //-----------------------------------------------------------//

        //--------------------Queue Operations-------------------------------//
        //these methods handle adding, peeking, receiving, and deleting messages in a queue
        public async Task AddMessageAsync(string queueName, string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }

        public async Task<List<string>> PeekMessagesAsync(string queueName, int maxMessages = 10)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            var messages = await queueClient.PeekMessagesAsync(maxMessages);
            var result = new List<string>();
            foreach (PeekedMessage msg in messages.Value)
            {
                result.Add(msg.MessageText);
            }
            return result;
        }

        public async Task<string> ReceiveAndDeleteMessageAsync(string queueName)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            var messages = await queueClient.ReceiveMessagesAsync(1);
            if (messages.Value.Length > 0)
            {
                var message = messages.Value[0];
                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                return message.MessageText;
            }
            return null;
        }
        // //-----------------------------------------------------------//
    }
    //---------------------------------------------------------------//
}
//----------------------------------------------------------------------//
//END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 22 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */

/*
 * @author:Kylan Chart Frittelli
 * @file: TableStorageService.cs
 * @since: 22/08/2025
 * @function: This service provides access to Azure Table Storage.
 */

//------------------namespace--------------------//
namespace ABCRetailWebApp.Services;
using System.Threading.Tasks;
using Azure.Data.Tables;

//--------------TableStorageService-----------------//
//this service provides access to Azure Table Storage
public class TableStorageService
{
    private readonly TableServiceClient _serviceClient;

    //----------------Constructor-------------------------------//
    //this constructor initializes the TableServiceClient with the connection string
    public TableStorageService(string connectionString)
    {
        _serviceClient = new TableServiceClient(connectionString);
    }
    //-----------------------------------------------------------//

    //--------------------GetTableClient-------------------------------//
    //this method retrieves a TableClient for the specified table name
    public TableClient GetTableClient(string tableName)
    {
        return _serviceClient.GetTableClient(tableName);
    }
    //-----------------------------------------------------------//
}
//------------------------------------------------------//

//-------------------------------------------------------//

//END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 22 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */

/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: HomeController.cs
 * @since [Updated: 22/08/2025]
 * @function: Controller for managing home page 
   and file operations in the ABC Retail Web Application.
 */

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ABCRetailWebApp.Models;
using ABCRetailWebApp.Services;

//---------------namespace------------------------------------------//
namespace ABCRetailWebApp.Controllers;

//-------------------HomeController-------------------------------//
public class HomeController : Controller
{
    //these are the services and logger used in the controller
    private readonly ILogger<HomeController> _logger;
    private readonly BlobStorageService _blobService;
    private readonly QueueStorageService _queueService;
    private readonly FileStorageService _fileService;

    //----------------Constructor-------------------------------//
    //this constructor initializes the services and logger via dependency injection
    public HomeController(
        ILogger<HomeController> logger,
        BlobStorageService blobService,
        QueueStorageService queueService,
        FileStorageService fileService)
    {
        _logger = logger;
        _blobService = blobService;
        _queueService = queueService;
        _fileService = fileService;
    }
    //-----------------------------------------------------------//

    //--------------------Index and Privacy-------------------------------//
    //these actions return the views for the home page and privacy policy
    public IActionResult Index() => View();
    public IActionResult Privacy() => View();
    //-----------------------------------------------------------//

    //--------------------Cache and Error-------------------------------//
    //this action handles errors and sets cache settings
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] //cache settings
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    //-----------------------------------------------------------//

    //--------------------File Operations-------------------------------//
    [HttpPost]
    //this action handles file uploads to Azure Blob Storage
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file is { Length: > 0 })
        {
            const string containerName = "retailimages";
            using var stream = file.OpenReadStream();
            await _blobService.UploadFileAsync(containerName, file.FileName, stream);
        }
        return RedirectToAction(nameof(BlobFiles));
    }

    [HttpGet]
    //this action retrieves a list of files from Azure Blob Storage
    public async Task<IActionResult> BlobFiles()
    {
        const string containerName = "retailimages";
        var files = await _blobService.ListFilesAsync(containerName);
        return View(files);
    }

    [HttpGet]
    //this action downloads a file from Azure Blob Storage
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        const string containerName = "retailimages";
        var stream = await _blobService.DownloadFileAsync(containerName, fileName);
        return File(stream, "application/octet-stream", fileName);
    }
    //-----------------------------------------------------------//

    //--------------------Queue Operations-------------------------------//
    [HttpPost]
    //this action adds a message to an Azure Queue Storage
    public async Task<IActionResult> AddQueueMessage(string message)
    {
        const string queueName = "orders";
        if (!string.IsNullOrWhiteSpace(message))
            await _queueService.AddMessageAsync(queueName, message);

        return RedirectToAction(nameof(QueueMessages));
    }
    [HttpGet]
    //this action retrieves messages from an Azure Queue Storage
    public async Task<IActionResult> QueueMessages()
    {
        const string queueName = "orders";
        var messages = await _queueService.PeekMessagesAsync(queueName);
        return View(messages);
    }
    //------------------------------------------------------------//

    //--------------------File Operations for Contracts-------------------------------//
    [HttpPost]
    //this action handles file uploads for contracts to Azure File Storage
    public async Task<IActionResult> UploadContract(IFormFile file)
    {
        const string shareName = "contracts";
        const string directoryName = "";
        if (file is { Length: > 0 })
        {
            using var stream = file.OpenReadStream();
            await _fileService.UploadFileAsync(shareName, directoryName, file.FileName, stream);
        }
        return RedirectToAction(nameof(FileContracts));
     }

        [HttpGet]
    //this action retrieves a list of files from Azure File Storage for contracts
    public async Task<IActionResult> FileContracts()
        {
            const string shareName = "contracts";
            const string directoryName = "";
            var files = await _fileService.ListFilesAsync(shareName, directoryName);
            return View(files);
        }

    [HttpGet]
    //this action downloads a contract file from Azure File Storage
    public async Task<IActionResult> DownloadContract(string fileName)
    {
        const string shareName = "contracts";
        const string directoryName = "";
        var stream = await _fileService.DownloadFileAsync(shareName, directoryName, fileName);
        return File(stream, "application/octet-stream", fileName);
    }
    //-------------------------------------------------------------------------//
}
//--------------------------------------------------------------//
//END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 22 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */

/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: ProductsController.cs
 * @since [Updated: 22/08/2025]
 * @function: Controller for managing products in the ABC Retail Web Application.
 */

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography.Xml;
using ABCRetailWebApp.Models;
using ABCRetailWebApp.Services;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using static System.Net.WebRequestMethods;

//---------------namespace------------------------------------------//
namespace ABCRetailWebApp.Controllers
{
    //-------------------ProductsController-------------------------------//
    //controller for managing products
    public class ProductsController : Controller
    {
        private readonly TableStorageService _tableSvc;
        private const string TableName = "Products";

        //----------------Constructor-------------------------------//
        //constructor to inject the TableStorageService
        public ProductsController(TableStorageService tableSvc)
        {
            _tableSvc = tableSvc;
        }
        //-----------------------------------------------------------//

        //--------------------GetClient-------------------------------//
        //helper method to get the TableClient for the Products table
        private TableClient GetClient()
        {
            var client = _tableSvc.GetTableClient(TableName);
            client.CreateIfNotExists(); //bootstrap the table if missing
            return client;
        }
        //-----------------------------------------------------------//

        //---------------Index-------------------------------//
        // GET: /Products
        //this action retrieves all products from the table storage
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var items = new List<Product>();

            await foreach (var p in client.QueryAsync<Product>(maxPerPage: 100))
                items.Add(p);

            //sorted newest first by Timestamp
            items = items
                .OrderByDescending(p => p.Timestamp ?? DateTimeOffset.MinValue)
                .ToList();

            return View(items);
        }
        //-----------------------------------------------------------//

        //---------------Create-------------------------------//
        // GET: /Products/Create
        // this action returns a view to create a new product
        [HttpGet]
        public IActionResult Create()
        {
            // defaults to make adding 5 records fast
            return View(new Product
            {
                PartitionKey = "Retail",                //group all rows
                RowKey = Guid.NewGuid().ToString("N")   //unique id
            });
        }
        //-----------------------------------------------------------//

        //---------------Create (POST)-------------------------------//
        // POST: /Products/Create
        // this action handles the form submission to create a new product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = GetClient();
            await client.UpsertEntityAsync(model, TableUpdateMode.Replace);
            return RedirectToAction(nameof(Index));
        }
        //-----------------------------------------------------------//

        //--------------Delete-------------------------------//
        // POST: /Products/Delete
        // this action handles the deletion of a product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            var client = GetClient();
            await client.DeleteEntityAsync(partitionKey, rowKey);
            return RedirectToAction(nameof(Index));
        }
        //-----------------------------------------------------------//
    }
    //-----------------------------------------------------------//
}
//-----------------------------------------------------------//
//END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 22 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */

@model List<string>

<h2>Upload Image (Blob Storage)</h2>
<form method="post" enctype="multipart/form-data" asp-action="UploadFile">
    <input type="file" name="file" />
    <button type="submit">Upload</button>
</form>

<h2>Stored Images (Blob Storage)</h2>
<ul>
    @foreach (var file in Model)
    {
        <li>
            @file |
            <a asp-action="DownloadFile" asp-route-fileName="@file">Download</a>
        </li>
    }
</ul>

@model List<string>

<h2>Upload Document (File Share)</h2>
<form method="post" enctype="multipart/form-data" asp-action="UploadContract">
    <input type="file" name="file" />
    <button type="submit">Upload</button>
</form>

<h2>Stored Documents (File Share)</h2>
<ul>
    @foreach (var file in Model)
    {
        <li>
            @file |
            <a asp-action="DownloadContract" asp-route-fileName="@file">Download</a>
        </li>
    }
</ul>

@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>
        <a asp-action="BlobFiles" asp-controller="Home" class="btn btn-primary">
            Manage Images (Blob Storage)
        </a>
        <a asp-action="QueueMessages" asp-controller="Home" class="btn btn-secondary">
            Transactions & Inventory (Queue Storage)
        </a>
        <a asp-action="FileContracts" asp-controller="Home" class="btn btn-info">
            Manage Documents (File Storage)
        </a>
        <a asp-action="Index" asp-controller="Products" class="btn btn-warning">
            Manage Products (Table Storage)
        </a>
    </p>
    <p>Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>

@{
    ViewData["Title"] = "Privacy Policy";
}
<h1>@ViewData["Title"]</h1>

<p>Use this page to detail your site's privacy policy.</p>

@model List<string>

<h2>Add Transactions/Inventory (Queue Storage) </h2>
<form method="post" asp-action="AddQueueMessage">
    <input type="text" name="message" placeholder="Enter a message (e.g. Processing order #123)" />
    <button type="submit">Add Message</button>
</form>

<h2>Queue Messages</h2>
<ul>
    @foreach (var msg in Model)
    {
        <li>@msg</li>
    }
</ul>

@model ABCRetailWebApp.Models.Product
@{
    ViewData["Title"] = "Add Product";
}

<h2>Add Product</h2>

<form asp-action="Create" method="post">
    <div class="mb-3">
        <label asp-for="PartitionKey" class="form-label"></label>
        <input asp-for="PartitionKey" class="form-control" />
    </div>

    <div class="mb-3">
        <label asp-for="RowKey" class="form-label"></label>
        <input asp-for="RowKey" class="form-control" />
        <div class="form-text">Keep default or paste your own ID.</div>
    </div>

    <div class="mb-3">
        <label asp-for="Name" class="form-label"></label>
        <input asp-for="Name" class="form-control" />
    </div>

    <div class="mb-3">
        <label asp-for="Description" class="form-label"></label>
        <input asp-for="Description" class="form-control" />
    </div>

    <div class="mb-3">
        <label asp-for="Price" class="form-label"></label>
        <input asp-for="Price" class="form-control" type="number" step="0.01" />
    </div>

    <button type="submit" class="btn btn-success">Save</button>
    <a asp-action="Index" class="btn btn-secondary">Back</a>
</form>

@model List<ABCRetailWebApp.Models.Product>

@{
    ViewData["Title"] = "Products (Table Storage)";
}

<h2>Products (Table Storage)</h2>

<p>
    <a asp-action="Create" class="btn btn-primary">Add Product</a>
</p>

@if (Model == null || !Model.Any())
{
    <p>No products yet.</p>
}
else
{
    <table class="table table-striped">
        <thead>
            <tr>
                <th>PartitionKey</th>
                <th>RowKey</th>
                <th>Name</th>
                <th>Description</th>
                <th>Price</th>
                <th>Timestamp</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
            @foreach (var p in Model)
            {
                <tr>
                    <td>@p.PartitionKey</td>
                    <td>@p.RowKey</td>
                    <td>@p.Name</td>
                    <td>@p.Description</td>
                    <td>@p.Price</td>
                    <td>@p.Timestamp</td>
                    <td>
                        <form asp-action="Delete" method="post" style="display:inline">
                            <input type="hidden" name="partitionKey" value="@p.PartitionKey" />
                            <input type="hidden" name="rowKey" value="@p.RowKey" />
                            <button type="submit" class="btn btn-sm btn-danger">Delete</button>
                        </form>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}

/*
 * @author: Kylan Chart Frittelli
 * @file: appsettings.json
 * @since: [Updated: 22/08/2025]
 * @function: This file contains the configuration settings for the application.
*/


{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "AzureStorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=mycldv6211blobstorage;AccountKey=4qglUZILpACGtlD5W0C6CeE5JMbqt1Bs77Kp47432csYwcmVCvgqn1yJSvJBiSK6PJ4Ku/Qnmeem+AStXguH0w==;EndpointSuffix=core.windows.net"
}

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 22 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */

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
