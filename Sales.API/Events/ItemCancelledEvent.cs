namespace Sales.API.Events
{
    public record ItemCancelledEvent(Guid SaleId, Guid ItemId, DateTime CancelledAt);
}
