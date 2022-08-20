using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OrderItemsReserver
{
    public static class DeliveryOrderProcessor
    {
        [FunctionName("DeliveryOrderProcessor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "my-database",
                collectionName: "my-container",
                ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<Order>(requestBody);
            Order order = new Order()
            {
                ShipAddress = data?.ShipAddress,
                FinalPrice = data?.FinalPrice,
                Items = data?.Items,
            };

            if (data != null)
            {
                // Add a JSON document to the output container.
                await documentsOut.AddAsync(new
                {
                    // create a random ID
                    id = System.Guid.NewGuid().ToString(),
                    shipAddress = order.ShipAddress,
                    finalPrice = order.FinalPrice,
                    items = order.Items
                });
            }

            string responseMessage = data == null
                ? "Add body data to save order."
                : "JSON file saved successfully.";

            return new OkObjectResult(responseMessage);
        }
    }

    public class Order
    {
        public Address ShipAddress { get; set; }
        public decimal FinalPrice { get; set; }
        public List<string> Items { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}
