using BelanjaYuk.API.Dtos.Product;
using BelanjaYuk.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/v1/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly BelanjaYukDbContext _context;
    public ProductController(BelanjaYukDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts([FromQuery] string? searchQuery)
    {
        var query = _context.MsProducts
            .Where(p => p.IsActive)
            .Join(_context.LtCategories,
                  product => product.IdCategory,
                  category => category.IdCategory,
                  (product, category) => new { product, category });
        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(pc =>
                pc.product.ProductName.Contains(searchQuery) ||
                pc.product.ProductDesc.Contains(searchQuery)
            );
        }
        var products = await query
            .Select(pc => new ProductSummaryDto
            {
                IdProduct = pc.product.IdProduct,
                ProductName = pc.product.ProductName,
                Price = pc.product.Price,
                DiscountProduct = pc.product.DiscountProduct,
                PriceAfterDiscount = pc.product.Price - (pc.product.Price * pc.product.DiscountProduct / 100),
                CategoryName = pc.category.CategoryName,
                AvgRating = _context.TrBuyerTransactionDetails
                            .Where(d => d.IdProduct == pc.product.IdProduct && d.Rating > 0)
                            .Select(d => (decimal?)d.Rating).Average() ?? 0
            })
            .ToListAsync();
        return Ok(products);
    }
    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByCategory(string categoryId, [FromQuery] string? searchQuery)
    {
        var query = _context.MsProducts
            .Where(p => p.IsActive && p.IdCategory == categoryId)
            .Join(_context.LtCategories,
                  product => product.IdCategory,
                  category => category.IdCategory,
                  (product, category) => new { product, category });
        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(pc =>
                pc.product.ProductName.Contains(searchQuery) ||
                pc.product.ProductDesc.Contains(searchQuery)
            );
        }
        var products = await query
            .Select(pc => new ProductSummaryDto
            {
                IdProduct = pc.product.IdProduct,
                ProductName = pc.product.ProductName,
                Price = pc.product.Price,
                DiscountProduct = pc.product.DiscountProduct,
                PriceAfterDiscount = pc.product.Price - (pc.product.Price * pc.product.DiscountProduct / 100),
                CategoryName = pc.category.CategoryName,
                AvgRating = _context.TrBuyerTransactionDetails
                            .Where(d => d.IdProduct == pc.product.IdProduct && d.Rating > 0)
                            .Select(d => (decimal?)d.Rating).Average() ?? 0
            })
            .ToListAsync();
        if (!products.Any())
        {
            return NotFound("Tidak ada produk untuk kriteria ini.");
        }
        return Ok(products);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productDto)
    {       
        var seller = await _context.MsUserSellers
            .FirstOrDefaultAsync(s => s.IdUser == productDto.UserId);
        if (seller == null)
        {
            return Forbid("Anda bukan seller terdaftar.");
        }
        var newProduct = new MsProduct
        {
            IdProduct = Guid.NewGuid().ToString(),
            IdUserSeller = seller.IdUserSeller,
            IdCategory = productDto.IdKategori,
            ProductName = productDto.NamaBarang,
            ProductDesc = productDto.DeskripsiBarang,
            Price = productDto.Harga,
            DiscountProduct = productDto.Diskon,
            Qty = productDto.Stok,
            IsActive = true,
            DateIn = DateTime.UtcNow,
            UserIn = productDto.UserId,
            DateUp = DateTime.UtcNow,
            UserUp = productDto.UserId
        };
        try
        {
            _context.MsProducts.Add(newProduct);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllProducts), new { id = newProduct.IdProduct }, newProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Gagal menyimpan produk: {ex.Message}");
        }
    }
    [HttpPost("update/{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductCreateDto productDto)
    {
        var product = await _context.MsProducts.FindAsync(id);
        if (product == null)
        {
            return NotFound("Produk tidak ditemukan.");
        }

        var seller = await _context.MsUserSellers
            .FirstOrDefaultAsync(s => s.IdUser == productDto.UserId);
        if (seller == null || product.IdUserSeller != seller.IdUserSeller)
        {
            return Forbid("Anda tidak memiliki izin untuk mengubah produk ini.");
        }

        // Update product fields
        product.IdCategory = productDto.IdKategori;
        product.ProductName = productDto.NamaBarang;
        product.ProductDesc = productDto.DeskripsiBarang;
        product.Price = productDto.Harga;
        product.DiscountProduct = productDto.Diskon;
        product.Qty = productDto.Stok;
        product.DateUp = DateTime.UtcNow;
        product.UserUp = productDto.UserId;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Gagal mengupdate produk: {ex.Message}");
        }
    }

    [HttpDelete("{userId}/{id}")]
    public async Task<IActionResult> DeleteProduct(string userId, string id)
    {
        
        var product = await _context.MsProducts.FindAsync(id);
        if (product == null)
        {
            return NotFound("Produk tidak ditemukan.");
        }
        var seller = await _context.MsUserSellers.FirstOrDefaultAsync(s => s.IdUser == userId);
        if (seller == null || product.IdUserSeller != seller.IdUserSeller)
        {
            return Forbid("Anda tidak memiliki izin untuk menghapus produk ini.");
        }
        product.IsActive = false;
        product.DateUp = DateTime.UtcNow;
        product.UserUp = userId;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}