using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Dtos.Seller
{
    public class RegisterSellerDto
    {
        [Required]
        public string userId { get; set; }
        [Required(ErrorMessage = "Nama Toko wajib diisi")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Nama Toko harus 3-40 karakter")]
        [RegularExpression(@"^[a-zA-Z0-9\s-]+$", ErrorMessage = "Nama Toko hanya boleh huruf, angka, spasi, dan tanda hubung (-)")]
        public string NamaToko { get; set; }
        [RegularExpression(@"^(08|\+628)\d{8,11}$", ErrorMessage = "No. HP Toko tidak valid.")]
        public string? NoHpToko { get; set; }
        [Required(ErrorMessage = "URL Toko wajib diisi")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "URL Toko hanya boleh huruf kecil, angka, dan tanda hubung")]
        public string UrlToko { get; set; }
        [Required(ErrorMessage = "Deskripsi wajib diisi")]
        [StringLength(2000, MinimumLength = 30, ErrorMessage = "Deskripsi harus 30-2000 karakter")]
        public string Deskripsi { get; set; }
        [Required(ErrorMessage = "Alamat wajib diisi")]
        [MinLength(10, ErrorMessage = "Alamat minimal 10 karakter")]
        public string AlamatLengkap { get; set; }
    }
}