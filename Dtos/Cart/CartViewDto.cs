namespace BelanjaYuk.API.Dtos.Cart
{
    public class CartViewDto
    {
        public string IdBuyerCart { get; set; }
        public string IdProduct { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountProduct { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public decimal SubTotal { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}