using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace OrderItemsReserver
{
    public class OrderItemsReserver
    {
        [FunctionName("OrderItemsReserver")]
        public static async Task Run(
            [ServiceBusTrigger("my-queue", Connection = "SBConnection")] string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            try
            {
                // To simulate error and test email sending.
                // throw new Exception("Failes to save");
                string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                string containerName = Environment.GetEnvironmentVariable("ContainerName");

                byte[] byteArray = Encoding.ASCII.GetBytes(myQueueItem);
                MemoryStream stream = new MemoryStream(byteArray);

                var blobClient = new BlobContainerClient(Connection, containerName);
                var blob = blobClient.GetBlobClient($"{Guid.NewGuid()}container");

                await blob.UploadAsync(stream);
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to save file with details of order | Error: {ex.Message}");

                HttpClient client = new HttpClient();
                var content = new StringContent(myQueueItem, Encoding.UTF8, "application/json");
                _ = client.PostAsync("https://eshopemaillogicapp.azurewebsites.net:443/api/eshopOnWebLogicApp/triggers/manual/invoke?api-version=2022-05-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=6Y7zLue709jSp91qawxfWXGT7Sp3TQJsJ0ex5TzAWYs", content);
            }
        }
    }
}
