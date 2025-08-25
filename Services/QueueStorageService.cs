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