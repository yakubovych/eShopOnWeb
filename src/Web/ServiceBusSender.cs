using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Threading.Tasks;

public class ServiceBusSender
{
    private readonly ServiceBusClient _client;
    private readonly Azure.Messaging.ServiceBus.ServiceBusSender _clientSender;
    private const string QUEUE_NAME = "my-queue";

    public ServiceBusSender(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ServiceBusConnectionString");
        _client = new ServiceBusClient(connectionString);
        _clientSender = _client.CreateSender(QUEUE_NAME);
    }

    public async Task SendMessage(List<ReservedOrder> reservedOrder)
    {
        string messagePayload = JsonSerializer.Serialize(reservedOrder);
        ServiceBusMessage message = new ServiceBusMessage(messagePayload);
        await _clientSender.SendMessageAsync(message).ConfigureAwait(false);
    }
}
