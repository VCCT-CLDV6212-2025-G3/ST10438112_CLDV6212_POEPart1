/*
@author: Kylan Chart Frittelli ST10438112
@file: DiagConfig.cs
@since [Updated: 06/10/25]
@function: returns the length of the configured 'StorageAccountConnection' environment variable
*/

//----------------------using directives----------------------------//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
//------------------------------------------------------------------//

namespace ABCRetail.Functions.Functions
{
    //----------------------DiagConfig class----------------------------//
    public class DiagConfig
    {
        //----------------------Run method----------------------------//
        [Function("DiagConfig")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "diag/config")] HttpRequestData req)
        {
            //read environment variable
            var cs = Environment.GetEnvironmentVariable("StorageAccountConnection");

            //return length of connection string for quick diagnostics
            var res = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await res.WriteStringAsync($"StorageAccountConnection length: {cs?.Length ?? 0}");
            return res;
        }
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