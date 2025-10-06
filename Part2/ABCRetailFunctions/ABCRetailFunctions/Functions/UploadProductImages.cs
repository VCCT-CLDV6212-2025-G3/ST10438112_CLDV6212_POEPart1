/*
@author: Kylan Chart Frittelli ST10438112
@file: UploadProductImage.cs
@since [Updated: 06/10/25]
@function: uploads images to blob container 'retailimages' and generates a temporary sas url
*/

//----------------------using directives----------------------------//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
//------------------------------------------------------------------//

namespace ABCRetail.Functions
{
    //----------------------UploadProductImage class----------------------------//
    public class UploadProductImage
    {
        private const string ContainerName = "retailimages";
        private readonly BlobServiceClient _blobService;

        //----------------------constructor----------------------------//
        public UploadProductImage(BlobServiceClient blobService)
        {
            _blobService = blobService;
        }
        //--------------------------------------------------------------//

        //----------------------Run method----------------------------//
        [Function("UploadProductImage")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post",
                Route = "upload-image/{fileName?}")]
            HttpRequestData req,
            string? fileName,
            FunctionContext ctx)
        {
            //retrieve logger for current context
            var log = ctx.GetLogger(nameof(UploadProductImage));

            //determine content type or default to octet stream
            var contentType = req.Headers.TryGetValues("Content-Type", out var cts)
                ? cts.FirstOrDefault() ?? "application/octet-stream"
                : "application/octet-stream";

            //if filename missing, generate one using detected extension
            if (string.IsNullOrWhiteSpace(fileName))
            {
                var ext = GuessExtension(contentType);
                fileName = $"{Guid.NewGuid():N}{ext}";
            }

            try
            {
                //----------------------container operations----------------------------//
                var container = _blobService.GetBlobContainerClient(ContainerName);
                await container.CreateIfNotExistsAsync();
                await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
                //--------------------------------------------------------------//

                //----------------------upload blob----------------------------//
                var blob = container.GetBlobClient(fileName);
                await blob.UploadAsync(
                    content: req.Body,
                    options: new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
                    },
                    cancellationToken: ctx.CancellationToken);
                //--------------------------------------------------------------//

                //----------------------generate sas url----------------------------//
                var url = blob.Uri.ToString();
                var expiresOn = DateTimeOffset.UtcNow.AddHours(1);
                var sasUri = blob.GenerateSasUri(
                    Azure.Storage.Sas.BlobSasPermissions.Read, expiresOn);
                //--------------------------------------------------------------//

                //----------------------return response----------------------------//
                var ok = req.CreateResponse(HttpStatusCode.OK);
                await ok.WriteAsJsonAsync(new
                {
                    fileName,
                    contentType,
                    url = sasUri.ToString(),
                    expiresOn
                });
                return ok;
                //--------------------------------------------------------------//
            }
            catch (Exception ex)
            {
                //handle any upload or connection failures
                log.LogError(ex, "Failed to upload image: {Message}", ex.Message);

                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteAsJsonAsync(new
                {
                    error = "Upload failed",
                    detail = ex.Message
                });
                return bad;
            }
        }
        //--------------------------------------------------------------//

        //----------------------GuessExtension method----------------------------//
        private static string GuessExtension(string contentType) => contentType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/webp" => ".webp",
            "image/bmp" => ".bmp",
            _ => ".bin"
        };
        //--------------------------------------------------------------//
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