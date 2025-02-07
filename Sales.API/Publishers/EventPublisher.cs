using Sales.API.Events;
using Sales.API.Interfaces;

namespace Sales.API.Publishers
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IRabbitMQProducer _rabbitMqPublisher;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(IRabbitMQProducer rabbitMqPublisher, ILogger<EventPublisher> logger)
        {
            _rabbitMqPublisher = rabbitMqPublisher;
            _logger = logger;
        }

        public async Task PublishSaleCreatedEvent(SaleCreatedEvent saleEvent)
        {
            await PublishEvent("sales.exchange", "sales.created", saleEvent);
        }

        public async Task PublishSaleModifiedEvent(SaleModifiedEvent saleEvent)
        {
            await PublishEvent("sales.exchange", "sales.modified", saleEvent);
        }

        public async Task PublishSaleCancelledEvent(SaleCancelledEvent saleEvent)
        {
            await PublishEvent("sales.exchange", "sales.cancelled", saleEvent);
        }

        public async Task PublishItemCancelledEvent(ItemCancelledEvent itemEvent)
        {
            await PublishEvent("sales.exchange", "items.cancelled", itemEvent);
        }

        private async Task PublishEvent<T>(string exchange, string routingKey, T eventMessage)
        {
            try
            {
                var messageBody = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(eventMessage);
                await _rabbitMqPublisher.Publish(messageBody);

                _logger.LogInformation($"Event published to exchange '{exchange}' with routing key '{routingKey}': {eventMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to publish event to exchange '{exchange}'.");
            }
        }
    }
}
