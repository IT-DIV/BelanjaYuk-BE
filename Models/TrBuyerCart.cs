using BelanjaYuk.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TrBuyerCart
{
    [Key]
    public string IdBuyerCart { get; set; } = string.Empty;
    public int Qty { get; set; }
    public DateTime DateIn { get; set; }
    public string UserIn { get; set; } = string.Empty;
    public DateTime DateUp { get; set; }
    public string UserUp { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    [Column("IdUser")]
    public string IdUser { get; set; } = string.Empty;
    [ForeignKey("IdUser")]
    public virtual MsUser User { get; set; } = null!;
    [Column("IdProduct")]
    public string IdProduct { get; set; } = string.Empty;
    [ForeignKey("IdProduct")]
    public virtual MsProduct Product { get; set; } = null!;
}