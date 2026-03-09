namespace BankingApi.Services;

using BankingApi.Models;
using BankingApi.Dtos;
using BankingApi.Data;
using Microsoft.EntityFrameworkCore;
using BankingApi.Exceptions;
using BankingApi.Enums;
using Hangfire;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

public class AccountService
{
    private readonly AppDbContext _context;
    private readonly ParseOwnerGuidService _ownerGuidService;
    private readonly ILogger _logger;

    public AccountService(AppDbContext context, ParseOwnerGuidService ownerGuidService, ILogger<AccountService> logger){
        _context = context;
        _ownerGuidService = ownerGuidService;
        _logger = logger;
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
        var transaction = Transaction.CreateTransaction(request.Amount, null, account, null, TransactionType.Deposit, oldBalance, account.Balance, TransactionStatus.Processed);
        account.Transactions.Add(transaction);
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

        if ((account.Balance - account.ReservedBalance) < request.Amount)
        {
            throw new BadRequestException("Not enough Balance!");
        }

        decimal oldBalance = account.Balance;
        account.Withdraw(request.Amount);
        var transaction = Transaction.CreateTransaction(request.Amount, null, account, null, TransactionType.Withdraw, oldBalance, account.Balance, TransactionStatus.Processed);
        account.Transactions.Add(transaction);
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

    public async Task<AccountStatementResponse> Statement(Guid accountId, int page, int pageSize)
    {
        var ownerGuid = _ownerGuidService.GetUserId();
        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == ownerGuid);

        if (account == null)
        {
            throw new NotFoundException("BankAccount not found!");
        }

        var transactions = await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new AccountStatementTransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Timestamp = t.Timestamp,
                Type = t.Type.ToString()
            })
            .ToListAsync();

        return new AccountStatementResponse
        {
            AccountId = account.Id,
            OwnerId = account.OwnerId,
            Transactions = transactions
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
        if (request.Amount <= 0)
            throw new BadRequestException("Please enter a valid Amount");

        var ownerGuid = _ownerGuidService.GetUserId();
        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerId == ownerGuid);

        if (account == null)
            throw new NotFoundException("Account for your User not found!");

        if ((account.Balance - account.ReservedBalance) < request.Amount)
            throw new BadRequestException("Not enought Balance on your Account!");
        

        var targetAccount = await _context.BankAccounts.SingleOrDefaultAsync(a => a.Id == request.TargetAccountGuid);

        if (targetAccount == null)
            throw new NotFoundException("Target Account not Found!");

        if (!request.InstantTransfer)
            return await CreateScheduledTransfer(request.Amount, account, targetAccount);

        return await CreateInstantTransfer(request.Amount, account, targetAccount);
    }

    private async Task<AccountTransferResponse> CreateScheduledTransfer(decimal amount, BankAccount account, BankAccount targetAccount)
    {
        var sourceNewBalance = account.Balance - amount;
        var sourceTransaction = Transaction
            .CreateTransaction(amount, 0m, account, targetAccount, TransactionType.ScheduledTransferSend, account.Balance, sourceNewBalance, TransactionStatus.Pending);

        var targetNewBalance = targetAccount.Balance + amount;
        var targetTransaction = Transaction
           .CreateTransaction(amount, 0m, targetAccount, account, TransactionType.TransferReceive, targetAccount.Balance, targetNewBalance, TransactionStatus.Pending);
        
        account.Transactions.Add(sourceTransaction);
        account.Reserve(amount);

        targetAccount.Transactions.Add(targetTransaction);
        await _context.SaveChangesAsync();

        BackgroundJob.Schedule<AccountService>(
            x => x.ExecuteScheduledTransfer(sourceTransaction.Id, targetTransaction.Id),
            TimeSpan.FromSeconds(15));

        return new AccountTransferResponse
        {
            AccountId = account.Id,
            Amount = sourceTransaction.Amount,
            NewBalance = sourceTransaction.NewBalance,
            TargetAccountId = sourceTransaction.TargetAccount.Id,
            Status = sourceTransaction.Status
        };
    }

    private async Task<AccountTransferResponse> CreateInstantTransfer(decimal amount, BankAccount account, BankAccount targetAccount)
    {
        var fee = account.CalcFee(amount);
        var newBalance = account.CalcNewBalance(amount, fee);
        var instantTransaction = Transaction
            .CreateTransaction(amount, fee, account, targetAccount, TransactionType.InstantTransferSend, account.Balance, newBalance, TransactionStatus.Processed);
        account.Transactions.Add(instantTransaction);

        var targetNewBalance = targetAccount.Balance + amount;
        var targetTransaction = Transaction
            .CreateTransaction(amount, 0, targetAccount, account, TransactionType.TransferReceive, targetAccount.Balance, targetNewBalance, TransactionStatus.Processed);
        targetAccount.Transactions.Add(targetTransaction);

        account.TransferSend(amount + fee);
        targetAccount.TransferReceive(amount);
        await _context.SaveChangesAsync();

        return new AccountTransferResponse
        {
            AccountId = account.Id,
            TargetAccountId = targetAccount.Id,
            Amount = instantTransaction.Amount + (instantTransaction.Fee ?? 0m),
            NewBalance = instantTransaction.NewBalance,
            Fee = instantTransaction.Fee,
            Status = instantTransaction.Status
        };
    }

    public async Task ExecuteScheduledTransfer(Guid sourceTransactionGuid, Guid targetTransactionGuid)
    {
        _logger.LogInformation("Starting Scheduled Transaction for: " + sourceTransactionGuid);
        var sourceTransaction = await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.TargetAccount)
            .FirstOrDefaultAsync(t => t.Id == sourceTransactionGuid);

        var targetTransaction = await _context.Transactions
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == targetTransactionGuid);

        if (sourceTransaction == null)
            throw new NotFoundException("Source transaction not found");

        if (targetTransaction == null)
            throw new NotFoundException("Target transaction not found");

        sourceTransaction.Account.TransferSend(sourceTransaction.Amount);
        sourceTransaction.Account.Unreserve(sourceTransaction.Amount);
        sourceTransaction.TargetAccount.TransferReceive(targetTransaction.Amount);

        sourceTransaction.Status = TransactionStatus.Processed;
        targetTransaction.Status = TransactionStatus.Processed;

        await _context.SaveChangesAsync();
    }
}