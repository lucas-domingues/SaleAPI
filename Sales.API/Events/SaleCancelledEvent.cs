namespace Sales.API.Events
{
    public record SaleCancelledEvent(int SaleId, DateTime CancelledAt);
}
