using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Sales.API.Interfaces;

namespace Sales.API.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMQProducer> _logger;

        public RabbitMQProducer(IConnection connection, ILogger<RabbitMQProducer> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        
        public async Task Publish<T>(T message)
        {
            
            using var channel =  await _connection.CreateChannelAsync();
            
            await channel.QueueDeclareAsync("sale", exclusive: false);
            
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: "", routingKey: "sale", body: body);
        }
    }
}
