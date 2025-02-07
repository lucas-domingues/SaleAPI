using RabbitMQ.Client;
using Sales.API.Interfaces;
using Sales.API.RabbitMQ;

namespace Sales.API.Extensions
{
    public static class RabbitMqConnectionFactoryExtensions
    {
        public static IServiceCollection AddRabbitMqConnectionFactory(this IServiceCollection services)
        {
            services.AddSingleton<RabbitMqConnectionFactory>();
            services.AddSingleton<IConnection>(provider =>
            {
                var factory = provider.GetRequiredService<RabbitMqConnectionFactory>();
                return factory.GetConnectionAsync().GetAwaiter().GetResult();
            });
            services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();

            return services;
        }
    }
}
