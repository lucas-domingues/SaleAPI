using Sales.API.Events;

namespace Sales.API.Publishers
{
    public interface IEventPublisher
    {
        Task PublishSaleCreatedEvent(SaleCreatedEvent saleEvent);
        Task PublishSaleModifiedEvent(SaleModifiedEvent saleEvent);
        Task PublishSaleCancelledEvent(SaleCancelledEvent saleEvent);
        Task PublishItemCancelledEvent(ItemCancelledEvent itemEvent);
    }
}
