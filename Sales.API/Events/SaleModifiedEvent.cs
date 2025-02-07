namespace Sales.API.Events
{
    public record SaleModifiedEvent(int SaleId, DateTime ModifiedAt, decimal NewTotalAmount);
}
