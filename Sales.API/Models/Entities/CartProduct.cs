namespace Sales.API.Models.Entities
{
    public class CartProduct
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        // Nova propriedade para armazenar o desconto
        public decimal Discount { get; set; } = 0.00m;
    }
}
