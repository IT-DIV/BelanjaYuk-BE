using System.ComponentModel.DataAnnotations;

public class LoginUserDto
{
    [Required]
    public string EmailOrPhone { get; set; }
    [Required]
    public string Password { get; set; }
}