using BankingApi.Enums;
namespace BankingApi.Models;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; }
    public TransactionType Type { get; }
    public BankAccount Account { get; }
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public Transaction() { }
    public Transaction(decimal amount, BankAccount account, TransactionType type)
    {
        Amount = amount;
        Account = account;
        Type = type;
    }
}