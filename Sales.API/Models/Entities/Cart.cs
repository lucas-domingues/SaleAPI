namespace Sales.API.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public List<CartProduct> Products { get; set; }

        public decimal TotalPrice { get; set; } = 0.00m;
    }
}
