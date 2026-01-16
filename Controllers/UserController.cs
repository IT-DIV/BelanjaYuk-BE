using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BelanjaYuk.API.Models;

[Route("api/v1/user")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly BelanjaYukDbContext _context;

    public UserController(BelanjaYukDbContext context)
    {
        _context = context;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        // Get userId from JWT token claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

        if (userIdClaim == null)
        {
            return Unauthorized(new { message = "Token tidak valid." });
        }

        var userId = userIdClaim.Value;

        // Get user info with gender
        var user = await _context.MsUsers
            .Where(u => u.IdUser == userId && u.IsActive)
            .Join(_context.LtGenders,
                  user => user.IdGender,
                  gender => gender.IdGender,
                  (user, gender) => new
                  {
                      user.IdUser,
                      user.UserName,
                      user.Email,
                      user.FirstName,
                      user.LastName,
                      user.PhoneNumber,
                      user.DOB,
                      user.IdGender,
                      GenderName = gender.GenderName
                  })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound(new { message = "User tidak ditemukan." });
        }

        // Get seller info if exists
        var seller = await _context.MsUserSellers
            .Where(s => s.IdUser == userId && s.IsActive)
            .Select(s => new
            {
                s.IdUserSeller,
                s.SellerName,
                s.SellerDesc,
                s.SellerCode,
                s.PhoneNumber,
                s.Address
            })
            .FirstOrDefaultAsync();

        // Get user addresses
        var addresses = await _context.TrHomeAddresses
            .Where(a => a.IdUser == userId && a.IsActive)
            .Select(a => new
            {
                a.IdHomeAddress,
                a.Provinsi,
                KotaKabupaten = a.KotaKabupaten,
                a.Kecamatan,
                a.KodePos,
                a.HomeAddressDesc,
                a.IsPrimaryAddress
            })
            .OrderByDescending(a => a.IsPrimaryAddress)
            .ToListAsync();

        var roles = new List<string> { "buyer" };
        if (seller != null)
        {
            roles.Add("seller");
        }

        return Ok(new
        {
            idUser = user.IdUser,
            username = user.UserName,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName,
            phoneNumber = user.PhoneNumber,
            dob = user.DOB,
            gender = new
            {
                idGender = user.IdGender,
                genderName = user.GenderName
            },
            roles = roles,
            isSeller = seller != null,
            seller = seller,
            addresses = addresses
        });
    }
}
