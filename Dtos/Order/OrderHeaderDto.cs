namespace BelanjaYuk.API.Dtos.Order
{
    public class OrderHeaderDto
    {
        public string IdBuyerTransaction { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal FinalPrice { get; set; }
        public string PaymentName { get; set; }
        public List<OrderDetailDto> Products { get; set; } = new List<OrderDetailDto>();
    }
}