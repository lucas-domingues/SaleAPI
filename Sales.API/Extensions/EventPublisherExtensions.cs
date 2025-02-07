using Sales.API.Publishers;

namespace Sales.API.Extensions
{
    public static class EventPublisherExtensions
    {
        public static IServiceCollection AddEventPublisher(this IServiceCollection services)
        {
            services.AddSingleton<IEventPublisher, EventPublisher>();
            return services;
        }
    }
}
