using System.ComponentModel.DataAnnotations;

namespace BelanjaYuk.API.Models
{
    public class LtCategory
    {
        [Key]
        public string IdCategory { get; set; }
        public string CategoryName { get; set; }
        public DateTime DateIn { get; set; }
        public string UserIn { get; set; }
        public DateTime DateUp { get; set; }
        public string UserUp { get; set; }
        public bool IsActive { get; set; }
    }
}