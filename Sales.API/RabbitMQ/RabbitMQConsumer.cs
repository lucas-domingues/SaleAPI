//using ApiRabbitMq.Domain.Entities;
//using ApiRabbitMq.Domain.Interfaces;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Text;
//using System.Text.Json;

//namespace Sales.API.Messaging
//{
//    public class RabbitMQConsumer
//    {
//        private readonly IProductService _productService;

//        public RabbitMQConsumer(IProductService productService) 
//        {
//            _productService = productService;
//        }

//        public async Task StartAsync(string queueName)
//        {

//            var factory = new ConnectionFactory
//            {
//                HostName = "localhost", 
//                UserName = "guest",
//                Password = "guest"
//            };
//            var connection = await factory.CreateConnectionAsync();
//            using var channel = await connection.CreateChannelAsync();

//            await channel.QueueDeclareAsync("product", exclusive: false);

//            var consumer = new AsyncEventingBasicConsumer(channel);

//            consumer.ReceivedAsync += async (model, eventArgs) =>
//            {
//                var body = eventArgs.Body.ToArray();
//                var message = Encoding.UTF8.GetString(body);
//                Console.WriteLine($"Product message received: {message}");
//                try
//                {
//                    var product = JsonSerializer.Deserialize<Product>(message);
//                    if (product != null)
//                    {
//                         _productService.AddProduct(product);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Error processing message: {ex.Message}");
//                }

//            };

//            await channel.BasicConsumeAsync(queue: "product", autoAck: true, consumer: consumer);

//            Console.ReadKey();
//        }
//    }
//}
