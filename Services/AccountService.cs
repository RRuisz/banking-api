namespace BankingApi.Services;

using BankingApi.Models;
using BankingApi.Dtos;
using BankingApi.Data;
using Microsoft.EntityFrameworkCore;
using BankingApi.Exceptions;

public class AccountService
{
    private readonly AppDbContext _context;
    private readonly ParseOwnerGuidService _ownerGuidService;

    public AccountService(AppDbContext context, ParseOwnerGuidService ownerGuidService){
        _context = context;
        _ownerGuidService = ownerGuidService;
    }

    public async Task<List<BankAccount>> GetAll()
    {
        var ownerGuid = _ownerGuidService.GetUserId();
        var accounts = await _context.BankAccounts
            .Where(a => a.OwnerId == ownerGuid)
            .ToListAsync();

        return accounts;
    }

    public async Task<AccountCreateResponse> Create()
    {
        var ownerGuid = _ownerGuidService.GetUserId();
        var owner = await _context.Owners.FindAsync(ownerGuid);
        if (owner == null)
        {
            throw new NotFoundException("Owner not found");
        }

        var account = new BankAccount(ownerGuid);

        await _context.BankAccounts.AddAsync(account);
        await _context.SaveChangesAsync();
        
        return new AccountCreateResponse
        {
            Id = account.Id,
            Balance = account.Balance,
            OwnerId = account.OwnerId
        };
    }

    public async Task<AccountDepositResponse> Deposit(Guid accountId, AccountDepositRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new BadRequestException("Please enter a valid Amount!");
        }
        var ownerGuid = _ownerGuidService.GetUserId();
        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == ownerGuid);

        if (account == null)
        {
            throw new NotFoundException("BankAccount not found!");
        }

        decimal oldBalance = account.Balance;
        account.Deposit(request.Amount);
        await _context.SaveChangesAsync();

        return new AccountDepositResponse
        {
            Id = account.Id,
            OwnerId = account.OwnerId,
            NewBalance = account.Balance,
            OldBalance = oldBalance
        };
    }

    public async Task<AccountWithdrawResponse> Withdraw(Guid accountId, AccountWithdrawRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new BadRequestException("Please enter a valid Amount");
        }
        var ownerGuid = _ownerGuidService.GetUserId();
        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == ownerGuid);
        if (account == null)
        {
            throw new NotFoundException("BankAccount not found!");
        }

        if (account.Balance < request.Amount)
        {
            throw new BadRequestException("Not enough Balance!");
        }

        decimal oldBalance = account.Balance;
        account.Withdraw(request.Amount);
        await _context.SaveChangesAsync();

        return new AccountWithdrawResponse
        {
            Id = account.Id,
            OwnerId = account.OwnerId,
            Amount = request.Amount,
            OldBalance = oldBalance,
            NewBalance = account.Balance
        };
    }

    public async Task<AccountStatementResponse> Statement(Guid accountId)
    {
        var ownerGuid = _ownerGuidService.GetUserId();
        var account = await _context.BankAccounts
        .Include(a => a.Transactions)
        .FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == ownerGuid);

        if (account == null)
        {
            throw new NotFoundException("BankAccount not found!");
        }

        return new AccountStatementResponse
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

    }

    public async Task<AccountGetBalanceResponse> GetBalance(Guid accountId)
    {
        var ownerGuid = _ownerGuidService.GetUserId();
        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == ownerGuid);

        if (account == null)
        {
            throw new NotFoundException("Account for your User not found!");
        }

        return new AccountGetBalanceResponse
        {
            Id = account.Id,
            Balance = account.Balance
        };

    }

    public async Task<AccountTransferResponse> Transfer(Guid accountId, AccountTransferRequest request)
    {
        var ownerGuid = _ownerGuidService.GetUserId();
        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == ownerGuid);

        if (account == null)
        {
            throw new NotFoundException("Account for your User not found!");
        }

        if (account.Balance < request.Amount)
        {
            throw new BadRequestException("Not enought Balance on your Account!");
        }

        var targetAccount = await _context.BankAccounts.SingleOrDefaultAsync(a => a.Id == request.TargetAccountGuid);
        if (targetAccount == null)
        {
            throw new NotFoundException("Target Account not Found!");
        }

        account.Transfer(request.Amount, targetAccount);
        await _context.SaveChangesAsync();
        
        return new AccountTransferResponse
        {
            AccountId = account.Id,
            TargetAccountId = targetAccount.Id,
            Amount = request.Amount,
            NewBalance = account.Balance,
        };
    }
}