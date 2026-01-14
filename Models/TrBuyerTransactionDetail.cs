using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Models
{
    public class TrBuyerTransactionDetail
    {
        [Key]
        public string IdBuyerTransactionDetail { get; set; }
        public int Qty { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceProduct { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountProduct { get; set; }
        public int Rating { get; set; }
        public string RatingComment { get; set; }
        public DateTime DateIn { get; set; }
        public string UserIn { get; set; }
        public DateTime DateUp { get; set; }
        public string UserUp { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("TrBuyerTransaction")]
        public string IdBuyerTransaction { get; set; }
        [ForeignKey("MsProduct")]
        public string IdProduct { get; set; }
        [ForeignKey("IdProduct")]
        public virtual MsProduct Product { get; set; }
        [ForeignKey("IdBuyerTransaction")]
        public virtual TrBuyerTransaction TransactionHeader { get; set; }
    }
}