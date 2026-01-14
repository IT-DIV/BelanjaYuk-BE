using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Dtos.Cart
{
    public class UpdateQtyDto
    {
        [Required]
        public string userId { get; set; }
        [Required]
        public string IdBuyerCart { get; set; }
        [Required]
        [Range(1, 99, ErrorMessage = "Kuantitas minimal 1, maksimal 99")]
        public int NewQty { get; set; }
    }
}