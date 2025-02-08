using Microsoft.Extensions.Logging;
using Moq;
using Sales.API.Events;
using Sales.API.Interfaces;
using Sales.API.Publishers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sales.Tests.Tests.Publishers
{
    public class EventPublisherTests
    {
        private readonly Mock<IRabbitMQProducer> _rabbitMqPublisherMock;
        private readonly Mock<ILogger<EventPublisher>> _loggerMock;
        private readonly EventPublisher _eventPublisher;

        public EventPublisherTests()
        {
            _rabbitMqPublisherMock = new Mock<IRabbitMQProducer>();
            _loggerMock = new Mock<ILogger<EventPublisher>>();
            _eventPublisher = new EventPublisher(_rabbitMqPublisherMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task PublishSaleCreatedEvent_ShouldPublishEvent()
        {
            // Arrange
            var saleEvent = new SaleCreatedEvent(1, DateTime.UtcNow, 100.50m);
            var expectedMessageBody = JsonSerializer.SerializeToUtf8Bytes(saleEvent);

            // Act
            await _eventPublisher.PublishSaleCreatedEvent(saleEvent);

            // Assert
            _rabbitMqPublisherMock.Verify(p => p.Publish(expectedMessageBody), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Event published to exchange 'sales.exchange' with routing key 'sales.created': {saleEvent}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task PublishSaleModifiedEvent_ShouldPublishEvent()
        {
            // Arrange
            var saleEvent = new SaleModifiedEvent((int)1, DateTime.UtcNow, 200.75m );
            var expectedMessageBody = JsonSerializer.SerializeToUtf8Bytes(saleEvent);

            // Act
            await _eventPublisher.PublishSaleModifiedEvent(saleEvent);

            // Assert
            _rabbitMqPublisherMock.Verify(p => p.Publish(expectedMessageBody), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Event published to exchange 'sales.exchange' with routing key 'sales.modified': {saleEvent}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task PublishSaleCancelledEvent_ShouldPublishEvent()
        {
            // Arrange
            var saleEvent = new SaleCancelledEvent ((int)1, DateTime.UtcNow);
            var expectedMessageBody = JsonSerializer.SerializeToUtf8Bytes(saleEvent);

            // Act
            await _eventPublisher.PublishSaleCancelledEvent(saleEvent);

            // Assert
            _rabbitMqPublisherMock.Verify(p => p.Publish(expectedMessageBody), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Event published to exchange 'sales.exchange' with routing key 'sales.cancelled': {saleEvent}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task PublishItemCancelledEvent_ShouldPublishEvent()
        {
            // Arrange
            var itemEvent = new ItemCancelledEvent (Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
            var expectedMessageBody = JsonSerializer.SerializeToUtf8Bytes(itemEvent);

            // Act
            await _eventPublisher.PublishItemCancelledEvent(itemEvent);

            // Assert
            _rabbitMqPublisherMock.Verify(p => p.Publish(expectedMessageBody), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Event published to exchange 'sales.exchange' with routing key 'items.cancelled': {itemEvent}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task PublishEvent_ShouldLogError_WhenPublishingFails()
        {
            // Arrange
            var saleEvent = new SaleCreatedEvent((int)1, DateTime.UtcNow, 100.50m);
            var exception = new Exception("Test exception");

            _rabbitMqPublisherMock.Setup(p => p.Publish(It.IsAny<byte[]>())).ThrowsAsync(exception);

            // Act
            await _eventPublisher.PublishSaleCreatedEvent(saleEvent);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to publish event to exchange 'sales.exchange'.")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
