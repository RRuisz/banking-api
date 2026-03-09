namespace BankingApi.Models;

using BankingApi.Enums;

public class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public Guid AccountId { get; private set; }
    public BankAccount Account { get; set; } = null!;

    public Guid? TargetAccountId { get; private set; }
    public BankAccount? TargetAccount { get; set; }
    public decimal OldBalance { get; private set; }
    public decimal NewBalance { get; private set; }
    public decimal? Fee { get; private set; } = 0;
    public TransactionStatus Status { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Transaction() { }

    public static Transaction CreateTransaction(
        decimal amount,
        decimal? fee,
        BankAccount account,
        BankAccount? targetAccount,
        TransactionType type,
        decimal oldBalance,
        decimal newBalance,
        TransactionStatus status)
    {
        return new Transaction
        {
            Amount = amount,
            Fee = fee,
            Account = account,
            AccountId = account.Id,
            TargetAccount = targetAccount,
            Type = type,
            OldBalance = oldBalance,
            NewBalance = newBalance,
            Status = status
        };
    }
}