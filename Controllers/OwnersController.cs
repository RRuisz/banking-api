namespace BankingApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using BankingApi.Data;
using BankingApi.Dtos;
using BankingApi.Models;

[ApiController]
[Route("api/user")]
public class OwnersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OwnersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOwnerRequest request)
    {
        var owner = new Owner(request.FirstName, request.LastName);

        await _context.Owners.AddAsync(owner);
        await _context.SaveChangesAsync();

        return Ok(owner);
    }
}