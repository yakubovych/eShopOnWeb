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
        private static BlobServiceClient serviceClient;
        private static BlobContainerClient containerClient;
        private static int maxRetriesCount = 3;

        [FunctionName("OrderItemsReserver")]
        public static async Task Run(
            [ServiceBusTrigger("my-queue", Connection = "SBConnection")] string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            try
            {
                // To simulate error and test email sending.
                //throw new Exception("Failed to save");
                string connection = Environment.GetEnvironmentVariable("BlobConnection");
                string containerName = Environment.GetEnvironmentVariable("ContainerName");

                var blobClientOptions = new BlobClientOptions();
                blobClientOptions.Retry.MaxRetries = maxRetriesCount;

                serviceClient = new BlobServiceClient(connection, blobClientOptions);

                containerClient = serviceClient.GetBlobContainerClient(containerName);

                BlobClient blob = containerClient.GetBlobClient($"{Guid.NewGuid()}containerName");

                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(myQueueItem)))
                {
                    await blob.UploadAsync(ms);
                }                
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to save file with details of order | Error: {ex.Message}");

                HttpClient client = new HttpClient();
                var content = new StringContent(myQueueItem, Encoding.UTF8, "application/json");
                client.PostAsync("https://eshoporderreservelogicapp.azurewebsites.net:443/api/orderReserveLogic/triggers/manual/invoke?api-version=2022-05-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=p1elbcnCBtAXFbMa75Y-C91Wwb41WQMLH3KYnySbb-8", content);
            }
        }
    }
}
