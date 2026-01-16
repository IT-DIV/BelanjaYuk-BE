using BelanjaYuk.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/v1/products")]
[ApiController]
public class ProductImageController : ControllerBase
{
    private readonly BelanjaYukDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public ProductImageController(BelanjaYukDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    /// <summary>
    /// Upload one or multiple images for a product
    /// POST /api/v1/products/{productId}/images
    /// </summary>
    [HttpPost("{productId}/images")]
    [Authorize]
    public async Task<IActionResult> UploadProductImages(string productId, [FromForm] List<IFormFile> images, [FromForm] string userId)
    {
        // Validate product exists
        var product = await _context.MsProducts.FindAsync(productId);
        if (product == null)
        {
            return NotFound("Produk tidak ditemukan.");
        }

        // Verify user is the seller of this product
        var seller = await _context.MsUserSellers.FirstOrDefaultAsync(s => s.IdUser == userId);
        if (seller == null || product.IdUserSeller != seller.IdUserSeller)
        {
            return Forbid("Anda tidak memiliki izin untuk mengupload gambar produk ini.");
        }

        // Validate images
        if (images == null || !images.Any())
        {
            return BadRequest("Tidak ada gambar yang diupload.");
        }

        var uploadedImages = new List<object>();
        var errors = new List<string>();

        foreach (var image in images)
        {
            // Validate file size
            if (image.Length > _maxFileSize)
            {
                errors.Add($"{image.FileName}: Ukuran file terlalu besar (maksimal 5MB).");
                continue;
            }

            // Validate file extension
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                errors.Add($"{image.FileName}: Tipe file tidak didukung. Gunakan: jpg, jpeg, png, gif, webp.");
                continue;
            }

            try
            {
                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";

                // Create directory structure: wwwroot/uploads/products/{productId}/
                var productFolder = Path.Combine(_environment.WebRootPath, "uploads", "products", productId);
                Directory.CreateDirectory(productFolder);

                // Save file
                var filePath = Path.Combine(productFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Save to database (store relative URL path)
                var imageUrl = $"/uploads/products/{productId}/{fileName}";
                var productImage = new TrProductImage
                {
                    IdProductImages = Guid.NewGuid().ToString(),
                    IdProduct = productId,
                    ProductImage = imageUrl,
                    DateIn = DateTime.UtcNow,
                    UserIn = userId,
                    DateUp = DateTime.UtcNow,
                    UserUp = userId,
                    IsActive = true
                };

                _context.TrProductImages.Add(productImage);
                await _context.SaveChangesAsync();

                uploadedImages.Add(new
                {
                    idProductImage = productImage.IdProductImages,
                    imageUrl = imageUrl,
                    fileName = image.FileName
                });
            }
            catch (Exception ex)
            {
                errors.Add($"{image.FileName}: Gagal mengupload - {ex.Message}");
            }
        }

        return Ok(new
        {
            success = uploadedImages.Any(),
            uploadedCount = uploadedImages.Count,
            images = uploadedImages,
            errors = errors.Any() ? errors : null
        });
    }

    /// <summary>
    /// Get all images for a product
    /// GET /api/v1/products/{productId}/images
    /// </summary>
    [HttpGet("{productId}/images")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductImages(string productId)
    {
        var product = await _context.MsProducts.FindAsync(productId);
        if (product == null)
        {
            return NotFound("Produk tidak ditemukan.");
        }

        var images = await _context.TrProductImages
            .Where(img => img.IdProduct == productId && img.IsActive)
            .OrderBy(img => img.DateIn)
            .Select(img => new
            {
                idProductImage = img.IdProductImages,
                imageUrl = img.ProductImage,
                uploadedAt = img.DateIn
            })
            .ToListAsync();

        return Ok(new
        {
            productId = productId,
            productName = product.ProductName,
            imageCount = images.Count,
            images = images
        });
    }

    /// <summary>
    /// Delete a product image
    /// DELETE /api/v1/products/images/{imageId}
    /// </summary>
    [HttpDelete("images/{imageId}")]
    [Authorize]
    public async Task<IActionResult> DeleteProductImage(string imageId, [FromQuery] string userId)
    {
        var productImage = await _context.TrProductImages
            .Include(img => img.IdProductNavigation)
            .FirstOrDefaultAsync(img => img.IdProductImages == imageId);

        if (productImage == null)
        {
            return NotFound("Gambar tidak ditemukan.");
        }

        // Verify user is the seller of this product
        var product = await _context.MsProducts.FindAsync(productImage.IdProduct);
        var seller = await _context.MsUserSellers.FirstOrDefaultAsync(s => s.IdUser == userId);

        if (seller == null || product.IdUserSeller != seller.IdUserSeller)
        {
            return Forbid("Anda tidak memiliki izin untuk menghapus gambar ini.");
        }

        try
        {
            // Delete physical file
            var filePath = Path.Combine(_environment.WebRootPath, productImage.ProductImage.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Soft delete in database
            productImage.IsActive = false;
            productImage.DateUp = DateTime.UtcNow;
            productImage.UserUp = userId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Gambar berhasil dihapus." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Gagal menghapus gambar: {ex.Message}");
        }
    }
}
