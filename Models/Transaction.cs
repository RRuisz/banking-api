using BankingApi.Enums;
namespace BankingApi.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public Guid AccountId { get; private set; }
    public BankAccount Account { get; set; } = null!;
    public decimal OldBalance { get; private set; }
    public decimal NewBalance { get; private set; }
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public Transaction() { }
    public Transaction(decimal amount, BankAccount account, TransactionType type, decimal oldBalance, decimal newBalance)
    {
        Amount = amount;
        AccountId = account.Id;
        Account = account;
        Type = type;
        OldBalance = oldBalance;
        NewBalance = newBalance;
    }
}