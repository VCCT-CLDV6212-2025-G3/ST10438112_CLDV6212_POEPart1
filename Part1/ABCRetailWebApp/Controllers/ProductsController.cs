/*
 * @author: Kylan Chart Frittelli (ST10438112)
 * @file: ProductsController.cs
 * @since [Updated: 04/10/2025]
 * @function: Controller for managing products in the ABC Retail Web Application.
 */

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography.Xml;
using ABCRetailWebApp.Models;
using ABCRetailWebApp.Services;
using Azure;
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
        public async Task<IActionResult> Create(
        [Bind("Name,Description,Price,PartitionKey,RowKey")] Product model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = GetClient();

            if (string.IsNullOrWhiteSpace(model.PartitionKey))
                model.PartitionKey = "Products";

            if (string.IsNullOrWhiteSpace(model.RowKey))
                model.RowKey = Guid.NewGuid().ToString("N");

            try
            {
                //AddEntityAsync will throw a 409 if the same PartitionKey+RowKey already exists
                await client.AddEntityAsync(model);
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 409)
            {
                //duplicate key conflict
                ModelState.AddModelError("", "A product with the same key already exists.");
                return View(model);
            }
            catch (Azure.RequestFailedException ex)
            {
                //other potential Azure table issues
                ModelState.AddModelError("", $"Error saving product: {ex.Message}");
                return View(model);
            }

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
 * OpenAI, 2025.ChatGPT. [online] Available at: https://openai.com/chatgpt/ [Accessed 04 October 2025].
 *Github Inc., 2025.GitHub Copilot. [online] Available at: https://github.com [Accessed 22 August 2025].
 */
