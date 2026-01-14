using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Dtos.Order
{
    public class SubmitReviewDto
    {
        [Required]
        public string userId { get; set; }
        [Required]
        public string IdBuyerTransactionDetail { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(1000)]
        public string RatingComment { get; set; } = "";
    }
}