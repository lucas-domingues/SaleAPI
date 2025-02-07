using RabbitMQ.Client;
using System;

namespace Sales.API.RabbitMQ
{
    public class RabbitMqConnectionFactory : IDisposable
    {
        private readonly ILogger<RabbitMqConnectionFactory> _logger;
        private readonly IConfiguration _configuration;
        private IConnection _connection;

        public RabbitMqConnectionFactory(ILogger<RabbitMqConnectionFactory> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IConnection> GetConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                try
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = _configuration["RabbitMQ:HostName"],
                        UserName = _configuration["RabbitMQ:UserName"],
                        Password = _configuration["RabbitMQ:Password"],
                        VirtualHost = _configuration["RabbitMQ:VirtualHost"],
                        Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672")
                    };
                    _connection = await Task.Run(() => factory.CreateConnectionAsync());
                    _logger.LogInformation("RabbitMQ connection established successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create RabbitMQ connection.");
                    throw;
                }
            }
            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.IsOpen)
            {
                _connection.CloseAsync();
                _connection.Dispose();
                _logger.LogInformation("RabbitMQ connection disposed.");
            }
        }
    }
}