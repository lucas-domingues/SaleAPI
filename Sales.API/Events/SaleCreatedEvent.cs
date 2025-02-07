namespace Sales.API.Events
{
    public record SaleCreatedEvent(int SaleId, DateTime CreatedAt, decimal TotalAmount);
}
