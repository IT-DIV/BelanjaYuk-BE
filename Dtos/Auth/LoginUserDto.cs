using System.ComponentModel.DataAnnotations;

public class LoginUserDto
{
    [Required]
    public string EmailOrPhone { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}