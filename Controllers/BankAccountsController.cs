namespace BankingApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApi.Models;
using BankingApi.Dtos;
using BankingApi.Data;
using BankingApi.Enums;

[ApiController]
[Route("api/bankaccounts")]
public class AccountsController: ControllerBase
{
    private readonly AppDbContext _context;

    public AccountsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var accounts = await _context.BankAccounts.ToListAsync();
        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountRequest request)
    {
        var owner = await _context.Owners.FindAsync(request.OwnerGuid);
        if (owner == null)
        {
            return NotFound("Owner not found");
        }

        var account = new BankAccount(request.OwnerGuid);

        await _context.BankAccounts.AddAsync(account);
        await _context.SaveChangesAsync();
        var response = new BankAccountResponse{
            Id = account.Id,
            Balance = account.Balance,
            OwnerId = account.OwnerId
        };

        return Ok(response);
    }
}