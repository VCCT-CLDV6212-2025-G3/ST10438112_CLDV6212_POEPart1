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
