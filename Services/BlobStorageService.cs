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
            await containerClient.CreateIfNotExistsAsync();
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