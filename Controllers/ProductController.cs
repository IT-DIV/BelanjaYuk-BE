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
    private readonly IWebHostEnvironment _environment;
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public ProductController(BelanjaYukDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
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
            .Select(pc => new
            {
                IdProduct = pc.product.IdProduct,
                ProductName = pc.product.ProductName,
                Price = pc.product.Price,
                DiscountProduct = pc.product.DiscountProduct,
                PriceAfterDiscount = pc.product.Price - (pc.product.Price * pc.product.DiscountProduct / 100),
                CategoryName = pc.category.CategoryName,
                AvgRating = _context.TrBuyerTransactionDetails
                            .Where(d => d.IdProduct == pc.product.IdProduct && d.Rating > 0)
                            .Select(d => (decimal?)d.Rating).Average() ?? 0,
                Images = _context.TrProductImages
                            .Where(img => img.IdProduct == pc.product.IdProduct && img.IsActive)
                            .Select(img => img.ProductImage)
                            .ToList()
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
            .Select(pc => new
            {
                IdProduct = pc.product.IdProduct,
                ProductName = pc.product.ProductName,
                Price = pc.product.Price,
                DiscountProduct = pc.product.DiscountProduct,
                PriceAfterDiscount = pc.product.Price - (pc.product.Price * pc.product.DiscountProduct / 100),
                CategoryName = pc.category.CategoryName,
                AvgRating = _context.TrBuyerTransactionDetails
                            .Where(d => d.IdProduct == pc.product.IdProduct && d.Rating > 0)
                            .Select(d => (decimal?)d.Rating).Average() ?? 0,
                Images = _context.TrProductImages
                            .Where(img => img.IdProduct == pc.product.IdProduct && img.IsActive)
                            .Select(img => img.ProductImage)
                            .ToList()
            })
            .ToListAsync();
        if (!products.Any())
        {
            return NotFound("Tidak ada produk untuk kriteria ini.");
        }
        return Ok(products);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromForm] string UserId,
        [FromForm] string NamaBarang,
        [FromForm] string DeskripsiBarang,
        [FromForm] string IdKategori,
        [FromForm] decimal Harga,
        [FromForm] decimal Diskon,
        [FromForm] int Stok,
        [FromForm] List<IFormFile>? images)
    {
        var seller = await _context.MsUserSellers
            .FirstOrDefaultAsync(s => s.IdUser == UserId);
        if (seller == null)
        {
            return StatusCode(403, new { message = "Anda bukan seller terdaftar." });
        }

        var newProduct = new MsProduct
        {
            IdProduct = Guid.NewGuid().ToString(),
            IdUserSeller = seller.IdUserSeller,
            IdCategory = IdKategori,
            ProductName = NamaBarang,
            ProductDesc = DeskripsiBarang,
            Price = Harga,
            DiscountProduct = Diskon,
            Qty = Stok,
            IsActive = true,
            DateIn = DateTime.UtcNow,
            UserIn = UserId,
            DateUp = DateTime.UtcNow,
            UserUp = UserId
        };

        try
        {
            _context.MsProducts.Add(newProduct);
            await _context.SaveChangesAsync();

            // Upload images if provided
            var uploadedImages = new List<object>();
            var imageErrors = new List<string>();

            if (images != null && images.Any())
            {
                foreach (var image in images)
                {
                    // Validate file size
                    if (image.Length > _maxFileSize)
                    {
                        imageErrors.Add($"{image.FileName}: Ukuran file terlalu besar (maksimal 5MB).");
                        continue;
                    }

                    // Validate file extension
                    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    if (!_allowedExtensions.Contains(extension))
                    {
                        imageErrors.Add($"{image.FileName}: Tipe file tidak didukung.");
                        continue;
                    }

                    try
                    {
                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}{extension}";

                        // Create directory structure
                        var productFolder = Path.Combine(_environment.WebRootPath, "uploads", "products", newProduct.IdProduct);
                        Directory.CreateDirectory(productFolder);

                        // Save file
                        var filePath = Path.Combine(productFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Save to database
                        var imageUrl = $"/uploads/products/{newProduct.IdProduct}/{fileName}";
                        var productImage = new TrProductImages
                        {
                            IdProductImages = Guid.NewGuid().ToString(),
                            IdProduct = newProduct.IdProduct,
                            ProductImage = imageUrl,
                            DateIn = DateTime.UtcNow,
                            UserIn = UserId,
                            DateUp = DateTime.UtcNow,
                            UserUp = UserId,
                            IsActive = true
                        };

                        _context.TrProductImages.Add(productImage);
                        await _context.SaveChangesAsync();

                        uploadedImages.Add(new
                        {
                            idProductImage = productImage.IdProductImages,
                            imageUrl = imageUrl
                        });
                    }
                    catch (Exception ex)
                    {
                        imageErrors.Add($"{image.FileName}: {ex.Message}");
                    }
                }
            }

            return Ok(new
            {
                message = "Produk berhasil dibuat",
                product = new
                {
                    idProduct = newProduct.IdProduct,
                    productName = newProduct.ProductName,
                    productDesc = newProduct.ProductDesc,
                    price = newProduct.Price,
                    discountProduct = newProduct.DiscountProduct,
                    qty = newProduct.Qty,
                    idCategory = newProduct.IdCategory
                },
                images = uploadedImages,
                imageErrors = imageErrors.Any() ? imageErrors : null
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Gagal menyimpan produk: {ex.Message}");
        }
    }
    [HttpPost("update/{id}")]
    public async Task<IActionResult> UpdateProduct(
        string id,
        [FromForm] string UserId,
        [FromForm] string NamaBarang,
        [FromForm] string DeskripsiBarang,
        [FromForm] string IdKategori,
        [FromForm] decimal Harga,
        [FromForm] decimal Diskon,
        [FromForm] int Stok,
        [FromForm] List<IFormFile>? images)
    {
        var product = await _context.MsProducts.FindAsync(id);
        if (product == null)
        {
            return NotFound("Produk tidak ditemukan.");
        }

        var seller = await _context.MsUserSellers
            .FirstOrDefaultAsync(s => s.IdUser == UserId);
        if (seller == null || product.IdUserSeller != seller.IdUserSeller)
        {
            return StatusCode(403, new { message = "Anda tidak memiliki izin untuk mengubah produk ini." });
        }

        // Update product fields
        product.IdCategory = IdKategori;
        product.ProductName = NamaBarang;
        product.ProductDesc = DeskripsiBarang;
        product.Price = Harga;
        product.DiscountProduct = Diskon;
        product.Qty = Stok;
        product.DateUp = DateTime.UtcNow;
        product.UserUp = UserId;

        try
        {
            await _context.SaveChangesAsync();

            // Upload new images if provided
            var uploadedImages = new List<object>();
            var imageErrors = new List<string>();

            if (images != null && images.Any())
            {
                foreach (var image in images)
                {
                    // Validate file size
                    if (image.Length > _maxFileSize)
                    {
                        imageErrors.Add($"{image.FileName}: Ukuran file terlalu besar (maksimal 5MB).");
                        continue;
                    }

                    // Validate file extension
                    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    if (!_allowedExtensions.Contains(extension))
                    {
                        imageErrors.Add($"{image.FileName}: Tipe file tidak didukung.");
                        continue;
                    }

                    try
                    {
                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}{extension}";

                        // Create directory structure
                        var productFolder = Path.Combine(_environment.WebRootPath, "uploads", "products", product.IdProduct);
                        Directory.CreateDirectory(productFolder);

                        // Save file
                        var filePath = Path.Combine(productFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Save to database
                        var imageUrl = $"/uploads/products/{product.IdProduct}/{fileName}";
                        var productImage = new TrProductImages
                        {
                            IdProductImages = Guid.NewGuid().ToString(),
                            IdProduct = product.IdProduct,
                            ProductImage = imageUrl,
                            DateIn = DateTime.UtcNow,
                            UserIn = UserId,
                            DateUp = DateTime.UtcNow,
                            UserUp = UserId,
                            IsActive = true
                        };

                        _context.TrProductImages.Add(productImage);
                        await _context.SaveChangesAsync();

                        uploadedImages.Add(new
                        {
                            idProductImage = productImage.IdProductImages,
                            imageUrl = imageUrl
                        });
                    }
                    catch (Exception ex)
                    {
                        imageErrors.Add($"{image.FileName}: {ex.Message}");
                    }
                }
            }

            // Get all current images
            var allImages = await _context.TrProductImages
                .Where(img => img.IdProduct == product.IdProduct && img.IsActive)
                .Select(img => new
                {
                    idProductImage = img.IdProductImages,
                    imageUrl = img.ProductImage
                })
                .ToListAsync();

            return Ok(new
            {
                message = "Produk berhasil diupdate",
                product = new
                {
                    idProduct = product.IdProduct,
                    productName = product.ProductName,
                    productDesc = product.ProductDesc,
                    price = product.Price,
                    discountProduct = product.DiscountProduct,
                    qty = product.Qty,
                    idCategory = product.IdCategory
                },
                newImages = uploadedImages,
                allImages = allImages,
                imageErrors = imageErrors.Any() ? imageErrors : null
            });
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
            return StatusCode(403, new { message = "Anda tidak memiliki izin untuk menghapus produk ini." });
        }
        product.IsActive = false;
        product.DateUp = DateTime.UtcNow;
        product.UserUp = userId;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}