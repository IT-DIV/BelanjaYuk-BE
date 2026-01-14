using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Models
{
    public class TrHomeAddress
    {
        [Key]
        public string IdHomeAddress { get; set; }
        public string Provinsi { get; set; }
        [Column("Kota/Kabupaten")]
        public string KotaKabupaten { get; set; }
        public string Kecamatan { get; set; }
        public string KodePos { get; set; }
        public string HomeAddressDesc { get; set; }
        public bool IsPrimaryAddress { get; set; }
        public DateTime DateIn { get; set; }
        public string UserIn { get; set; }
        public DateTime DateUp { get; set; }
        public string UserUp { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("MsUser")]
        public string IdUser { get; set; }
    }
}