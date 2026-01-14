using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Models
{
    public class MsProduct
    {
        [Key]
        public string IdProduct { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountProduct { get; set; }
        public int Qty { get; set; }
        public DateTime DateIn { get; set; }
        public string UserIn { get; set; }
        public DateTime DateUp { get; set; }
        public string UserUp { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("MsUserSeller")]
        public string IdUserSeller { get; set; }
        [ForeignKey("LtCategory")]
        public string IdCategory { get; set; }
    }
}