namespace BankingApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApi.Models;
using BankingApi.Dtos;
using BankingApi.Data;
using BankingApi.Enums;

[ApiController]
[Route("api/bankaccounts")]
public class AccountsController : ControllerBase
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
        var response = new CreateBankAccountResponse {
            Id = account.Id,
            Balance = account.Balance,
            OwnerId = account.OwnerId
        };

        return Ok(response);
    }

    [HttpPost("{accountId}/deposit")]
    public async Task<IActionResult> Deposit(Guid accountId, AccountDepositRequest request)
    {
        if (request.Amount <= 0)
        {
            return BadRequest("Please enter a valid Amount!");
        }

        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == request.OwnerGuid);
        
        if (account == null)
        {
            return NotFound("BankAccount not found!");
        }

        decimal oldBalance = account.Balance;
        account.Deposit(request.Amount);
        await _context.SaveChangesAsync();

        var response = new AccountDepositResponse
        {
            Id = account.Id,
            OwnerId = account.OwnerId,
            NewBalance = account.Balance,
            OldBalance = oldBalance
        };

        return Ok(response);
    }

    [HttpPost("{accountId}/withdraw")]
    public async Task<IActionResult> Withdraw(Guid accountId, AccountWithdrawRequest request)
    {
        if (request.Amount <= 0)
        {
            return BadRequest("Please enter a valid Amount");
        }

        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == request.OwnerGuid);
        if (account == null)
        {
            return NotFound("BankAccount not found!");
        }

        if (account.Balance < request.Amount)
        {
            return BadRequest("Not enough Balance!");
        }

        decimal oldBalance = account.Balance;
        account.Withdraw(request.Amount);
        await _context.SaveChangesAsync();

        var response = new AccountWithdrawResponse
        {
            Id = account.Id,
            OwnerId = account.OwnerId,
            Amount = request.Amount,
            OldBalance = oldBalance,
            NewBalance = account.Balance
        };

        return Ok(response);
    }

    [HttpGet("{accountId}/statement")]
    public async Task<IActionResult> Statement(Guid accountId, [FromQuery] Guid ownerGuid)
    {
        var account = await _context.BankAccounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == ownerGuid);
        
        if (account == null)
        {
            return NotFound("BankAccount not found!");
        }

        var response = new AccountStatementResponse
        {
            AccountId = account.Id,
            OwnerId = account.OwnerId,
            Transactions = account.Transactions
            .OrderByDescending(t => t.Timestamp)
            .Select(t => new AccountStatementTransactionResponse
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                OldBalance = t.OldBalance,
                NewBalance = t.NewBalance,
                Timestamp = t.Timestamp
            }).ToList()
        };

        return Ok(response);
    }
}