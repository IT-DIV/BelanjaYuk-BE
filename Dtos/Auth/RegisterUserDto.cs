using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Dtos.Auth
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Nama wajib diisi.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Nama hanya boleh huruf dan spasi.")]
        public string NamaLengkap { get; set; }
        [Required(ErrorMessage = "Username wajib diisi.")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Username harus 5-30 karakter")]
        [RegularExpression(@"^[a-z0-9]+$", ErrorMessage = "Gunakan huruf kecil dan angka saja.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Nomor HP wajib diisi.")]
        [RegularExpression(@"^(08|\+628)\d{8,11}$", ErrorMessage = "Nomor HP tidak valid.")]
        public string NoHP { get; set; }
        [Required(ErrorMessage = "Kata sandi wajib diisi.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Kata sandi minimal 8 karakter dan kombinasi huruf besar, kecil, angka.")]
        public string KataSandi { get; set; }
        [Required(ErrorMessage = "Konfirmasi sandi wajib diisi.")]
        [Compare("KataSandi", ErrorMessage = "Kata sandi tidak sama.")]
        public string KonfirmasiSandi { get; set; }
        [Required(ErrorMessage = "Tanggal lahir wajib diisi.")]
        public DateTime TanggalLahir { get; set; }
        [Required(ErrorMessage = "Jenis kelamin wajib diisi.")]
        public string IdGender { get; set; }
        public HomeAddressDto? AlamatUtama { get; set; } //
    }
    public class HomeAddressDto
    {
        public string? Provinsi { get; set; }
        public string? KotaKabupaten { get; set; }
        public string? Kecamatan { get; set; }
        public string? KodePos { get; set; }
        public string? AlamatLengkap { get; set; }
    }
}