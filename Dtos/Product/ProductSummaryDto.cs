namespace BelanjaYuk.API.Dtos.Product
{
    public class ProductSummaryDto
    {
        public string IdProduct { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountProduct { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string CategoryName { get; set; }
        public decimal AvgRating { get; set; }
        public int Qty { get; set; }
    }
}