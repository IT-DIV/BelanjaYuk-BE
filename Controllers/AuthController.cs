using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using BelanjaYuk.API.Models;
using BelanjaYuk.API.Dtos.Auth;
using Microsoft.Data.SqlClient;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly BelanjaYukDbContext _context;
    private readonly IConfiguration _configuration;
    public AuthController(BelanjaYukDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerDto)
    {
        if (registerDto.TanggalLahir > DateTime.Today)
        {
            return BadRequest("Format tanggal tidak valid. Tidak boleh lebih dari hari ini.");
        }
        if (registerDto.TanggalLahir > DateTime.Today.AddYears(-13))
        {
            return BadRequest("Minimal umur 13 tahun.");
        }

        // Check if email already exists
        if (await _context.MsUsers.AnyAsync(u => u.Email == registerDto.Email))
        {
            return BadRequest("Email sudah terdaftar.");
        }

        // Check if username already exists
        if (await _context.MsUsers.AnyAsync(u => u.UserName == registerDto.Username))
        {
            return BadRequest("Username sudah digunakan.");
        }

        // Check if phone number already exists
        if (await _context.MsUsers.AnyAsync(u => u.PhoneNumber == registerDto.NoHP))
        {
            return BadRequest("Nomor HP sudah terdaftar.");
        }
        var newUser = new MsUser
        {
            IdUser = Guid.NewGuid().ToString(),
            UserName = registerDto.Username,
            Email = registerDto.Email,
            PhoneNumber = registerDto.NoHP,
            FirstName = registerDto.NamaLengkap,
            LastName = "",
            DOB = registerDto.TanggalLahir,
            IdGender = registerDto.IdGender,
            IsActive = true,
            DateIn = DateTime.UtcNow,
            UserIn = "SYSTEM",
            DateUp = DateTime.UtcNow,
            UserUp = "SYSTEM"
        };
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.KataSandi);
        var newUserPassword = new MsUserPassword
        {
            IdUserPassword = Guid.NewGuid().ToString(),
            IdUser = newUser.IdUser,
            PasswordHash = hashedPassword,
            IsActive = true,
            DateIn = DateTime.UtcNow,
            UserIn = "SYSTEM",
            DateUp = DateTime.UtcNow,
            UserUp = "SYSTEM"
        };
        TrHomeAddress? newAddress = null;
        if (registerDto.AlamatUtama != null &&
            !string.IsNullOrEmpty(registerDto.AlamatUtama.AlamatLengkap))
        {
            newAddress = new TrHomeAddress
            {
                IdHomeAddress = Guid.NewGuid().ToString(),
                IdUser = newUser.IdUser,
                Provinsi = registerDto.AlamatUtama.Provinsi,
                KotaKabupaten = registerDto.AlamatUtama.KotaKabupaten,
                Kecamatan = registerDto.AlamatUtama.Kecamatan,
                KodePos = registerDto.AlamatUtama.KodePos,
                HomeAddressDesc = registerDto.AlamatUtama.AlamatLengkap,
                IsPrimaryAddress = true,
                IsActive = true,
                DateIn = DateTime.UtcNow,
                UserIn = "SYSTEM",
                DateUp = DateTime.UtcNow,
                UserUp = "SYSTEM"
            };
        }
        try
        {
            _context.MsUsers.Add(newUser);
            _context.MsUserPasswords.Add(newUserPassword);
            if (newAddress != null)
            {
                _context.TrHomeAddresses.Add(newAddress);
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(RegisterUser), new { id = newUser.IdUser }, newUser);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx &&
                (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                if (sqlEx.Message.Contains("UserName"))
                    return BadRequest("Username sudah digunakan.");
                if (sqlEx.Message.Contains("Email"))
                    return BadRequest("Email sudah terdaftar.");
                if (sqlEx.Message.Contains("PhoneNumber"))
                    return BadRequest("Nomor HP sudah terdaftar.");
            }
            return StatusCode(500, $"Terjadi error : {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Terjadi error : {ex.Message}");
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginDto)
    {
        var user = await _context.MsUsers
            .FirstOrDefaultAsync(u => u.Email == loginDto.EmailOrPhone || u.PhoneNumber == loginDto.EmailOrPhone);

        if (user == null)
        {
            return Unauthorized("Email, nomor HP, atau kata sandi salah.");
        }
        var userPassword = await _context.MsUserPasswords
            .FirstOrDefaultAsync(p => p.IdUser == user.IdUser && p.IsActive);
        if (userPassword == null)
        {
            return Unauthorized("Email, nomor HP, atau kata sandi salah.");
        }
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, userPassword.PasswordHash);
        if (!isPasswordValid)
        {
            return Unauthorized("Email, nomor HP, atau kata sandi salah.");
        }
         
        var seller = _context.MsUserSellers.Where(s => s.IdUser == user.IdUser).Select(a => new { a.SellerName}).FirstOrDefault();
        List<string> currentRoles = new List<string>();
        currentRoles.Add("buyer");

        List<string> currentRoles2 = new List<string>();
        currentRoles2.Add("buyer");
        currentRoles2.Add("seller");


        var expiration = DateTime.UtcNow.AddHours(2);
        var token = GenerateJwtToken(user.IdUser, user.UserName, (seller != null ? currentRoles2 : currentRoles), expiration);

        var returnResult = new UserTokenDto()
        {
            Token = token,
            IdUSer = user.IdUser,
            Expiration = expiration,
            Roles = (seller != null ? currentRoles2 : currentRoles),
            Username = user.UserName,
            StoreName = (seller != null ? seller.SellerName : ""),
        };


        return Ok(returnResult);
    }

    private string GenerateJwtToken(string userId, string username, List<string> roles, DateTime expiration)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
        var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");

        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Add roles as claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}