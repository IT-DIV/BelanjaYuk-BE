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

        bool alreadySeller = await _context.MsUserSellers.AnyAsync(s => s.IdUser == registerDto.userId);
        if (alreadySeller)
        {
            return BadRequest("User ini sudah terdaftar sebagai seller.");
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
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx &&
                (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                if (sqlEx.Message.Contains("SellerCode") || sqlEx.Message.Contains("UrlToko"))
                {
                    return BadRequest("URL toko sudah digunakan.");
                }
            }
            return StatusCode(500, $"Terjadi error internal: {ex.Message}");
        }
    }
    [HttpGet("my-products/{userId}")]
    public async Task<IActionResult> GetMyProducts(string userId, [FromQuery] string? searchQuery)
    {       
        var seller = await _context.MsUserSellers
            .FirstOrDefaultAsync(s => s.IdUser == userId);
        if (seller == null) return Forbid("Anda bukan seller terdaftar.");
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
            .Select(p => new ProductSummaryDto
            {
                IdProduct = p.IdProduct,
                ProductName = p.ProductName,
                Price = p.Price,
                DiscountProduct = p.DiscountProduct,
                PriceAfterDiscount = p.Price - (p.Price * p.DiscountProduct / 100),
                Qty = p.Qty
            })
            .OrderByDescending(p => p.IdProduct)
            .ToListAsync();
        return Ok(myProducts);
    }
}