using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Dtos.Product
{
    public class ProductCreateDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string NamaBarang { get; set; }
        public string DeskripsiBarang { get; set; }
        [Required]
        public string IdKategori { get; set; }
        [Required]
        public decimal Harga { get; set; }
        public decimal Diskon { get; set; } = 0;
        [Required]
        public int Stok { get; set; }
    }
}