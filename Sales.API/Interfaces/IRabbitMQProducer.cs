namespace Sales.API.Interfaces
{
    public interface IRabbitMQProducer
    {
        Task Publish<T>(T message);
    }
}
