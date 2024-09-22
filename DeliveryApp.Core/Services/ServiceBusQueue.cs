using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using DeliveryApp.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;


namespace DeliveryApp.Core.Services
{
    public class ServiceBusQueue : INavigationService, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusAdministrationClient _adminClient;
        private const string QueueName = "deliveryqueue";
        private readonly ILogger<ServiceBusQueue> _logger;

        public ServiceBusQueue(string connectionString, ILogger<ServiceBusQueue> logger)
        {
            _client = new ServiceBusClient(connectionString, new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });

            _adminClient = new ServiceBusAdministrationClient(connectionString);
            _logger = logger;

            try
            {
                CreateQueue().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating the queue.");
            }
        }

        public async Task CreateQueue()
        {
            try
            {
                if (!await _adminClient.QueueExistsAsync(QueueName))
                {
                    await _adminClient.CreateQueueAsync(QueueName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating the queue {QueueName}.", QueueName);
                throw;
            }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                var sender = _client.CreateSender(QueueName);
                await sender.SendMessageAsync(new ServiceBusMessage(BinaryData.FromString(message)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to the queue {QueueName}.", QueueName);
                throw;
            }
        }

        public async Task<IEnumerable<string>> ReceiveMassagesAsync()
        {
            try
            {
                var receiver = _client.CreateReceiver(QueueName, new ServiceBusReceiverOptions
                {
                    ReceiveMode = ServiceBusReceiveMode.PeekLock,
                });

                var messages = await receiver.PeekMessagesAsync(10);
                var list = new List<string>();

                foreach (var message in messages)
                {
                    list.Add(Encoding.UTF8.GetString(message.Body));
                }

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving messages from the queue {QueueName}.", QueueName);
                throw;
            }
        }

        public async Task SetupProcessor(Func<ProcessErrorEventArgs, Task> processError,
                                          Func<ProcessMessageEventArgs, Task> processMessage)
        {
            try
            {
                var processor = _client.CreateProcessor(QueueName, new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    ReceiveMode = ServiceBusReceiveMode.PeekLock,
                });

                processor.ProcessErrorAsync += processError;
                processor.ProcessMessageAsync += processMessage;

                await processor.StartProcessingAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up the message processor for queue {QueueName}.", QueueName);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _client.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing the ServiceBusClient.");
                throw;
            }
        }

    }
}
