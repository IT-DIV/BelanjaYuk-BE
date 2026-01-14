using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Dtos.Cart
{
    public class CheckoutDto
    {
        [Required]
        public string userId {  get; set; }
        [Required]
        public string IdPayment { get; set; }
    }
}