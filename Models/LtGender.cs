using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Models
{
    public class LtGender
    {
        [Key]
        public string IdGender { get; set; }
        public string GenderName { get; set; }
        public DateTime DateIn { get; set; }
        public string UserIn { get; set; }
        public DateTime DateUp { get; set; }
        public string UserUp { get; set; }
        public bool IsActive { get; set; }
    }
}