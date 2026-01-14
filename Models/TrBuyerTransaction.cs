using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Models
{
    public class TrBuyerTransaction
    {
        [Key]
        public string IdBuyerTransaction { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalPrice { get; set; }
        public int Rating { get; set; }
        public string RatingComment { get; set; }
        public DateTime DateIn { get; set; }
        public string UserIn { get; set; }
        public DateTime DateUp { get; set; }
        public string UserUp { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("MsUser")]
        public string IdUser { get; set; }
        [ForeignKey("LtPayment")]
        public string IdPayment { get; set; }
        [ForeignKey("IdPayment")] 
        public virtual LtPayment Payment { get; set; }
        public virtual ICollection<TrBuyerTransactionDetail> TransactionDetails { get; set; }
    }
}