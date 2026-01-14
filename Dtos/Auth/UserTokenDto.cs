public class UserTokenDto
{
    //public string Token { get; set; }
    public string IdUSer { get; set; }
    public DateTime Expiration { get; set; }
    public string Username { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public string? StoreName { get; set; }
}