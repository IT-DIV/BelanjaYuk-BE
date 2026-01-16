using BelanjaYuk.API.Dtos.Seller;
using BelanjaYuk.API.Dtos.Product;
using BelanjaYuk.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Data;
using Microsoft.Data.SqlClient;

[Route("api/v1/seller")]
[ApiController]
public class SellerController : ControllerBase
{
    private readonly BelanjaYukDbContext _context;
    public SellerController(BelanjaYukDbContext context)
    {
        _context = context;
    }
    [HttpPost("register")]
    public async Task<IActionResult> RegisterSeller([FromBody] RegisterSellerDto registerDto)
    {
        // Check if user already registered as seller
        bool alreadySeller = await _context.MsUserSellers.AnyAsync(s => s.IdUser == registerDto.userId);
        if (alreadySeller)
        {
            return BadRequest("User ini sudah terdaftar sebagai seller.");
        }

        // Check if seller name already exists
        bool sellerNameExists = await _context.MsUserSellers.AnyAsync(s => s.SellerName == registerDto.NamaToko && s.IsActive);
        if (sellerNameExists)
        {
            return BadRequest("Nama toko sudah digunakan.");
        }

        // Check if seller code (URL) already exists
        bool sellerCodeExists = await _context.MsUserSellers.AnyAsync(s => s.SellerCode == registerDto.UrlToko && s.IsActive);
        if (sellerCodeExists)
        {
            return BadRequest("URL toko sudah digunakan.");
        }

        // Check if phone number already exists (if provided)
        if (!string.IsNullOrEmpty(registerDto.NoHpToko))
        {
            bool phoneExists = await _context.MsUserSellers.AnyAsync(s => s.PhoneNumber == registerDto.NoHpToko && s.IsActive);
            if (phoneExists)
            {
                return BadRequest("Nomor HP toko sudah digunakan.");
            }
        }
        var newSeller = new MsUserSeller
        {
            IdUserSeller = Guid.NewGuid().ToString(),
            IdUser = registerDto.userId,
            SellerName = registerDto.NamaToko,
            SellerDesc = registerDto.Deskripsi,
            Address = registerDto.AlamatLengkap,
            SellerCode = registerDto.UrlToko,
            PhoneNumber = registerDto.NoHpToko ?? "",
            DateIn = DateTime.UtcNow,
            UserIn = registerDto.userId,
            DateUp = DateTime.UtcNow,
            UserUp = registerDto.userId,
            IsActive = true
        };
        try
        {
            _context.MsUserSellers.Add(newSeller);
            await _context.SaveChangesAsync();
            return Ok(newSeller);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Terjadi error internal: {ex.Message}");
        }
    }
    [HttpGet("my-products/{userId}")]
    public async Task<IActionResult> GetMyProducts(string userId, [FromQuery] string? searchQuery)
    {       
        var seller = await _context.MsUserSellers
            .FirstOrDefaultAsync(s => s.IdUser == userId);
        if (seller == null)
        {
            return StatusCode(403, new { message = "Anda bukan seller terdaftar." });
        }
        var query = _context.MsProducts
            .Where(p => p.IdUserSeller == seller.IdUserSeller && p.IsActive);
        if (!string.IsNullOrEmpty(searchQuery))
        {
            if (searchQuery.Length < 2)
            {
                return BadRequest("Input minimal 2 karakter.");
            }
            query = query.Where(p =>
                p.ProductName.Contains(searchQuery)
            );
        }
        var myProducts = await query
            .Select(p => new
            {
                IdProduct = p.IdProduct,
                ProductName = p.ProductName,
                Price = p.Price,
                DiscountProduct = p.DiscountProduct,
                PriceAfterDiscount = p.Price - (p.Price * p.DiscountProduct / 100),
                Qty = p.Qty,
                Images = _context.TrProductImages
                            .Where(img => img.IdProduct == p.IdProduct && img.IsActive)
                            .Select(img => img.ProductImage)
                            .ToList()
            })
            .OrderByDescending(p => p.IdProduct)
            .ToListAsync();
        return Ok(myProducts);
    }
}