using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Dtos.Cart
{
    public class CartAddItemDto
    {
        [Required]
        public string userId { get; set; }
        [Required]
        public string ProductId { get; set; }
        [Required]
        [Range(1, 99)]
        public int Quantity { get; set; } = 1;
    }
}