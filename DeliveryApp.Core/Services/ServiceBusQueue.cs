using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System.Text;

namespace DeliveryApp.Core.Services;

/// <summary>
/// Клас для роботи з повідомленнями
/// </summary>
public class ServiceBusQueue
{
    private readonly ServiceBusClient _client; 
    private readonly ServiceBusAdministrationClient _administrationClient;
    private readonly ServiceBusProcessor? _processor = null;
    private readonly string _queueName = "deliveryqueue";

    public ServiceBusQueue(string connectionString)
    {
        _client = new ServiceBusClient(connectionString, new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        });
        _administrationClient = new ServiceBusAdministrationClient(connectionString);
    }

    public async Task CreateQueue()
    {
        if(!await _administrationClient.QueueExistsAsync(_queueName)) {
            await _administrationClient.CreateQueueAsync(_queueName);
        }
    }

    public async Task SendMessageAsync(string message)
    {
        var queue = _client.CreateSender(_queueName);

        await queue.SendMessageAsync(new ServiceBusMessage()
        {
            Body = BinaryData.FromString(message),
        });
    }

    public async Task<IEnumerable<string>> ReceiveMessagesAsync()
    {
        var queue = _client.CreateReceiver(_queueName, new ServiceBusReceiverOptions()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
        });

        var messages = await queue.ReceiveMessagesAsync(10);

        var list = new List<string>();

        foreach (var message in messages)
        {
            list.Add(Encoding.UTF8.GetString(message.Body));
        }

        return list;
    }

    public async Task SetupProcessor(Func<ProcessErrorEventArgs, Task> processError,
        Func<ProcessMessageEventArgs, Task> processMessage)
    {
        var processor = _client.CreateProcessor(_queueName, new ServiceBusProcessorOptions()
        {
            AutoCompleteMessages = false,
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
        });

        processor.ProcessErrorAsync += processError;
        processor.ProcessMessageAsync += processMessage;

        await processor.StartProcessingAsync();
    }
}
