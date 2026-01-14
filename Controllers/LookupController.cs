using BelanjaYuk.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/v1/lookup")]
[ApiController]
public class LookupController : ControllerBase
{
    private readonly BelanjaYukDbContext _context;
    public LookupController(BelanjaYukDbContext context)
    {
        _context = context;
    }
    [HttpGet("genders")]
    public async Task<IActionResult> GetGenders()
    {
        var genders = await _context.LtGenders
             .Where(g => g.IsActive)
             .Select(g => new { g.IdGender, g.GenderName })
             .ToListAsync();

        return Ok(genders);
    }
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.LtCategories
            .Where(c => c.IsActive)
            .Select(c => new { c.IdCategory, c.CategoryName })
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

        return Ok(categories);
    }
    [HttpGet("payment-options")]
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
}