using BelanjaYuk.API.Dtos.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/v1/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly BelanjaYukDbContext _context;
    public OrderController(BelanjaYukDbContext context)
    {
        _context = context;
    }
    [HttpGet("cart/{userId}")]
    public async Task<IActionResult> GetMyOrders(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var transactions = await _context.TrBuyerTransactions
            .Where(t => t.IdUser == userId && t.IsActive)
            .OrderByDescending(t => t.DateIn)
            .Select(t => new OrderHeaderDto
            {
                IdBuyerTransaction = t.IdBuyerTransaction,
                TransactionDate = t.DateIn,
                FinalPrice = t.FinalPrice,
                PaymentName = t.Payment.PaymentName,
                Products = t.TransactionDetails
                    .Select(d => new OrderDetailDto
                    {
                        IdBuyerTransactionDetail = d.IdBuyerTransactionDetail,
                        IdProduct = d.IdProduct,
                        ProductName = d.Product.ProductName,
                        Qty = d.Qty,
                        PriceAtTransaction = d.PriceProduct,
                        Rating = d.Rating,
                        RatingComment = d.RatingComment,
                        Images = _context.TrProductImages
                                    .Where(img => img.IdProduct == d.IdProduct && img.IsActive)
                                    .Select(img => img.ProductImage)
                                    .ToList()
                    })
                    .ToList()
            })
            .ToListAsync();
        return Ok(transactions);
    }
    [HttpPost("review")]
    public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewDto reviewDto)
    {        
      
        var transactionDetail = await _context.TrBuyerTransactionDetails
            .Include(d => d.TransactionHeader)
            .FirstOrDefaultAsync(d => d.IdBuyerTransactionDetail == reviewDto.IdBuyerTransactionDetail);
        if (transactionDetail == null)
        {
            return NotFound("Detail pesanan tidak ditemukan.");
        }
        if (transactionDetail.TransactionHeader.IdUser != reviewDto.userId)
        {
            return StatusCode(403, new { message = "Anda tidak bisa mengulas barang milik orang lain." });
        }
        transactionDetail.Rating = reviewDto.Rating;
        transactionDetail.RatingComment = reviewDto.RatingComment;
        transactionDetail.DateUp = DateTime.UtcNow;
        transactionDetail.UserUp = reviewDto.userId;
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Ulasan berhasil dikirim." });
    }
}