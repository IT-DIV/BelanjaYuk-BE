using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Models
{
    public class MsUserSeller
    {
        [Key]
        public string IdUserSeller { get; set; }
        public string SellerName { get; set; }
        public string SellerDesc { get; set; }
        public string Address { get; set; }
        public string SellerCode { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateIn { get; set; }
        public string UserIn { get; set; }
        public DateTime DateUp { get; set; }
        public string UserUp { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("MsUser")]
        public string IdUser { get; set; }
    }
}