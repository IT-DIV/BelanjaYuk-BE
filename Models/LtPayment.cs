using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Models
{
    public class LtPayment
    {
        [Key]
        public string IdPayment { get; set; }
        public string PaymentName { get; set; }
        public DateTime DateIn { get; set; }
        public string UserIn { get; set; }
        public DateTime DateUp { get; set; }
        public string UserUp { get; set; }
        public bool IsActive { get; set; }
    }
}