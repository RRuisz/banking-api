namespace BankingApi.Models;

using BankingApi.Enums;

public class BankAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Balance { get; private set; }

    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; } = null!;

    public List<Transaction> Transactions { get; private set; } = [];

    private BankAccount() { }

    public BankAccount(Guid ownerId)
    {
        OwnerId = ownerId;
    }

    public void Deposit(decimal amount)
    {
        decimal oldBalance = Balance;
        Balance += amount;
        Transactions.Add(new Transaction(amount, this, TransactionType.Deposit, oldBalance, Balance));
    }

    public void Withdraw(decimal amount)
    {
        decimal oldBalance = Balance;
        Balance -= amount;
        Transactions.Add(new Transaction(amount, this, TransactionType.Withdraw, oldBalance, Balance));
    }
}