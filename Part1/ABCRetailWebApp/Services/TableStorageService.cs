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