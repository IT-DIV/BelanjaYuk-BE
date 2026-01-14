using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Models
{
    public class TrProductImage
    {
        [Key]
        public string IdProductImages { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string ProductImage { get; set; }
        public DateTime DateIn { get; set; }
        public string UserIn { get; set; }
        public DateTime DateUp { get; set; }
        public string UserUp { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("MsProduct")]
        public string IdProduct { get; set; }
    }
}