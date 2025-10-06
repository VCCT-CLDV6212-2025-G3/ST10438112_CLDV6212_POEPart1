/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: HomeController.cs
 * @since [Updated: 04/10/2025]
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
    //this action adds a message to an Azure Queue Storage
    [HttpPost]
    public async Task<IActionResult> AddQueueMessage(string message)
    {
        const string queueName = "orders";
        if (string.IsNullOrWhiteSpace(message)) return RedirectToAction(nameof(QueueMessages));

        try { using var _ = System.Text.Json.JsonDocument.Parse(message); }
        catch { TempData["Err"] = "Please submit valid JSON matching the transaction schema."; return RedirectToAction(nameof(QueueMessages)); }

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
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 04 October 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */