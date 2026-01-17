namespace BelanjaYuk.API.Dtos.Order
{
    public class OrderDetailDto
    {
        public string IdBuyerTransactionDetail { get; set; }
        public string IdProduct { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public decimal PriceAtTransaction { get; set; }
        public int Rating { get; set; }
        public string RatingComment { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}