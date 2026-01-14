using BelanjaYuk.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TrBuyerCart
{
    [Key]
    public string IdBuyerCart { get; set; }
    public int Qty { get; set; }
    public DateTime DateIn { get; set; }
    public string UserIn { get; set; }
    public DateTime DateUp { get; set; }
    public string UserUp { get; set; }
    public bool IsActive { get; set; }
    [Column("IdUser")]
    public string IdUser { get; set; }
    [ForeignKey("IdUser")]
    public virtual MsUser User { get; set; }
    [Column("IdProduct")]
    public string IdProduct { get; set; }
    [ForeignKey("IdProduct")]
    public virtual MsProduct Product { get; set; }
}