/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: FileStorageService.cs
 * @since [Updated: 26/08/2025]
 * @function: Service for managing file operations in Azure File Storage
 *  (File Storage Service runs into errors even after ChatGPT 
 *  and GitHub Copilot assistance as a last resort)
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

//-----------namespace------------------//
namespace ABCRetailWebApp.Services
{
    //--------------------FileStorageService--------------------//
    //manages file operations in Azure File Storage
    public class FileStorageService
    {
        private readonly ShareServiceClient _fileServiceClient;

        //--------------------Constructor--------------------//
        //initialises the ShareServiceClient with the connection string
        public FileStorageService(string connectionString)
        {
            _fileServiceClient = new ShareServiceClient(connectionString);
        }
        //----------------------------------------------------//

        //--------------------File Operations--------------------//
        public async Task UploadFileAsync(string shareName, string directoryName, string fileName, Stream fileStream)
        {
            if (string.IsNullOrWhiteSpace(shareName))
                throw new ArgumentException("shareName is required.", nameof(shareName));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName is required.", nameof(fileName));
            if (fileStream is null || fileStream.Length == 0)
                throw new ArgumentException("fileStream is empty.", nameof(fileStream));

            var shareClient = _fileServiceClient.GetShareClient(shareName);
            await shareClient.CreateIfNotExistsAsync();

            
            ShareDirectoryClient dirClient = string.IsNullOrWhiteSpace(directoryName)
                ? shareClient.GetRootDirectoryClient()
                : shareClient.GetDirectoryClient(directoryName);

            await dirClient.CreateIfNotExistsAsync();

            var fileClient = dirClient.GetFileClient(fileName);

            //create the file with its final size
            long totalLength = fileStream.Length;
            await fileClient.CreateAsync(totalLength);

           
            if (fileStream.CanSeek) fileStream.Seek(0, SeekOrigin.Begin);

            
            const int ChunkSize = 4 * 1024 * 1024;
            byte[] buffer = new byte[ChunkSize];
            long offset = 0;

            int read;
            while ((read = await fileStream.ReadAsync(buffer, 0, ChunkSize)) > 0)
            {
                using var ms = new MemoryStream(buffer, 0, read, writable: false);
                await fileClient.UploadRangeAsync(
                    new HttpRange(offset, read),
                    ms);

                offset += read;
            }
        }
        public async Task<Stream> DownloadFileAsync(string shareName, string directoryName, string fileName)
        {
            var shareClient = _fileServiceClient.GetShareClient(shareName);
            ShareDirectoryClient dirClient = string.IsNullOrWhiteSpace(directoryName)
                ? shareClient.GetRootDirectoryClient()
                : shareClient.GetDirectoryClient(directoryName);

            var fileClient = dirClient.GetFileClient(fileName);
            ShareFileDownloadInfo download = await fileClient.DownloadAsync();
            return download.Content;
        }

        public async Task<List<string>> ListFilesAsync(string shareName, string directoryName)
        {
            var shareClient = _fileServiceClient.GetShareClient(shareName);
            ShareDirectoryClient dirClient = string.IsNullOrWhiteSpace(directoryName)
                ? shareClient.GetRootDirectoryClient()
                : shareClient.GetDirectoryClient(directoryName);

            var result = new List<string>();
            await foreach (ShareFileItem item in dirClient.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory) result.Add(item.Name);
            }
            return result;
        }

        public async Task DeleteFileAsync(string shareName, string directoryName, string fileName)
        {
            var shareClient = _fileServiceClient.GetShareClient(shareName);
            ShareDirectoryClient dirClient = string.IsNullOrWhiteSpace(directoryName)
                ? shareClient.GetRootDirectoryClient()
                : shareClient.GetDirectoryClient(directoryName);

            var fileClient = dirClient.GetFileClient(fileName);
            await fileClient.DeleteIfExistsAsync();
        }
        //----------------------------------------------------//
    }
    //----------------------------------------------------//
}
//----------------------------------------------------//

//END OF FILE>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
 * Refrences:
 *Huawei Technologies, 2023. Cloud Computing Technologies. Hangzhou: Posts & Telecom Press.
 * Mrzyglód, K., 2022.Azure for Developers. 2nd ed.Birmingham: Packt Publishing.
 * Microsoft Corporation, 2022.The Developer’s Guide to Azure.Redmond: Microsoft Press.
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 26 August 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 26 August 2025].
 */