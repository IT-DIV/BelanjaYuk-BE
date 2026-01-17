using BelanjaYuk.API.Dtos.Cart;
using BelanjaYuk.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BelanjaYuk.API.Dtos;
using System.Data;

[Route("api/v1/cart")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly BelanjaYukDbContext _context;
    public CartController(BelanjaYukDbContext context)
    {
        _context = context;
    }
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] CartAddItemDto itemDto)
    {

        var cartItem = await _context.TrBuyerCarts
            .FirstOrDefaultAsync(c => c.IdUser == itemDto.userId && c.IdProduct == itemDto.ProductId && c.IsActive);
        if (cartItem != null)
        {
            cartItem.Qty += itemDto.Quantity;
            cartItem.DateUp = DateTime.UtcNow;
            cartItem.UserUp = itemDto.userId;
        }
        else
        {
            cartItem = new TrBuyerCart
            {
                IdBuyerCart = Guid.NewGuid().ToString(),
                IdUser = itemDto.userId,
                IdProduct = itemDto.ProductId,
                Qty = itemDto.Quantity,
                IsActive = true,
                DateIn = DateTime.UtcNow,
                UserIn = itemDto.userId,
                DateUp = DateTime.UtcNow,
                UserUp = itemDto.userId
            };
            _context.TrBuyerCarts.Add(cartItem);
        }
        await _context.SaveChangesAsync();
        return Ok(cartItem);
    }
    [HttpGet("cart/{userId}")]
    public async Task<IActionResult> GetCart(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var cartItems = await _context.TrBuyerCarts
            .Where(c => c.IdUser == userId && c.IsActive)
            .Join(_context.MsProducts,
                  cart => cart.IdProduct,
                  product => product.IdProduct,
                  (cart, product) => new CartViewDto
                  {
                      IdBuyerCart = cart.IdBuyerCart,
                      IdProduct = product.IdProduct,
                      ProductName = product.ProductName,
                      Qty = cart.Qty,
                      Price = product.Price,
                      DiscountProduct = product.DiscountProduct,
                      PriceAfterDiscount = product.Price - (product.Price * product.DiscountProduct / 100),
                      SubTotal = cart.Qty * (product.Price - (product.Price * product.DiscountProduct / 100)),
                      Images = _context.TrProductImages
                                  .Where(img => img.IdProduct == product.IdProduct && img.IsActive)
                                  .Select(img => img.ProductImage)
                                  .ToList()
                  })
            .ToListAsync();
        return Ok(cartItems);
    }
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
    {
        var cartItems = await _context.TrBuyerCarts
            .Where(c => c.IdUser == checkoutDto.userId && c.IsActive)
            .Include(c => c.Product)
            .ToListAsync();
        if (!cartItems.Any())
        {
            return BadRequest("Keranjang Anda kosong.");
        }
        decimal finalPrice = 0;
        foreach (var item in cartItems)
        {
            var priceAfterDiscount = item.Product.Price - (item.Product.Price * item.Product.DiscountProduct / 100);
            finalPrice += (priceAfterDiscount * item.Qty);
        }
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var newTransaction = new TrBuyerTransaction
                {
                    IdBuyerTransaction = Guid.NewGuid().ToString(),
                    IdUser = checkoutDto.userId,
                    IdPayment = checkoutDto.IdPayment,
                    FinalPrice = finalPrice,
                    Rating = 0,
                    RatingComment = "",
                    IsActive = true,
                    DateIn = DateTime.UtcNow,
                    UserIn = checkoutDto.userId,
                    DateUp = DateTime.UtcNow,
                    UserUp = checkoutDto.userId
                };
                _context.TrBuyerTransactions.Add(newTransaction);
                await _context.SaveChangesAsync();
                foreach (var item in cartItems)
                {
                    var priceAfterDiscount = item.Product.Price - (item.Product.Price * item.Product.DiscountProduct / 100);
                    var detail = new TrBuyerTransactionDetail
                    {
                        IdBuyerTransactionDetail = Guid.NewGuid().ToString(),
                        IdBuyerTransaction = newTransaction.IdBuyerTransaction,
                        IdProduct = item.IdProduct,
                        Qty = item.Qty,
                        PriceProduct = item.Product.Price,
                        DiscountProduct = item.Product.DiscountProduct,
                        Rating = 0,
                        RatingComment = "",
                        IsActive = true,
                        DateIn = DateTime.UtcNow,
                        UserIn = checkoutDto.userId,
                        DateUp = DateTime.UtcNow,
                        UserUp = checkoutDto.userId
                    };
                    _context.TrBuyerTransactionDetails.Add(detail);
                }
                foreach (var item in cartItems)
                {
                    item.IsActive = false;
                    item.UserUp = checkoutDto.userId;
                    item.DateUp = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { Message = "Checkout berhasil!", TransactionId = newTransaction.IdBuyerTransaction });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Checkout gagal: {ex.Message}");
            }
        }
    }
    [HttpGet("payment-options")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPaymentOptions()
    {
        var options = await _context.LtPayments
            .Where(p => p.IsActive)
            .Select(p => new PaymentDto
            {
                IdPayment = p.IdPayment,
                PaymentName = p.PaymentName
            })
            .ToListAsync();
        return Ok(options);
    }
    [HttpPost("update-quantity")]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQtyDto dto)
    {
      
        var cartItem = await _context.TrBuyerCarts.FirstOrDefaultAsync(c => c.IdBuyerCart == dto.IdBuyerCart && c.IdUser == dto.userId);
        if (cartItem == null)
        {
            return NotFound("Item tidak ditemukan di keranjang.");
        }
        cartItem.Qty = dto.NewQty;
        cartItem.DateUp = DateTime.UtcNow;
        cartItem.UserUp = dto.userId;
        await _context.SaveChangesAsync();
        return Ok(cartItem);
    }
    [HttpDelete("remove/{userId}/{idBuyerCart}")]
    public async Task<IActionResult> RemoveItem(string idBuyerCart, string userId)
    {
        var cartItem = await _context.TrBuyerCarts.FirstOrDefaultAsync(c => c.IdBuyerCart == idBuyerCart);
        if (cartItem == null)
        {
            return NotFound("Item tidak ditemukan di keranjang.");
        }
        cartItem.IsActive = false;
        cartItem.DateUp = DateTime.UtcNow;
        cartItem.UserUp = userId;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}