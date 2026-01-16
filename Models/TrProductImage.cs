using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Models
{
    [Table("TrProductimages")]
    public class TrProductImage
    {
        [Key]
        public string IdProductImages { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(MAX)")]
        public string ProductImage { get; set; } = string.Empty;
        public DateTime DateIn { get; set; }
        public string UserIn { get; set; } = string.Empty;
        public DateTime DateUp { get; set; }
        public string UserUp { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        [ForeignKey("MsProduct")]
        public string IdProduct { get; set; } = string.Empty;
    }
}