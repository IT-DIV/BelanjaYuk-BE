public class UserTokenDto
{
    public string Token { get; set; } = string.Empty;
    public string IdUSer { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
    public string? StoreName { get; set; }
}