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
        CreateTransaction(amount, TransactionType.Deposit, oldBalance);
    }

    public void Withdraw(decimal amount)
    {
        decimal oldBalance = Balance;
        Balance -= amount;
        CreateTransaction(amount, TransactionType.Withdraw, oldBalance);
    }

    public void Transfer(decimal amount, BankAccount targetAccount)
    {
        TransferSend(amount);
        targetAccount.TransferReceive(amount);
    }

    private void TransferSend(decimal amount)
    {
        var oldBalance = Balance;
        Balance -= amount;

        CreateTransaction(amount, TransactionType.TransferSend, oldBalance);
    }

    private void TransferReceive(decimal amount)
    {
        var oldBalance = Balance;
        Balance += amount;

        CreateTransaction(amount, TransactionType.TransferReceive, oldBalance);
    }

    private void CreateTransaction(decimal amount, TransactionType type, decimal oldBalance)
    {
        Transactions.Add(new Transaction(amount, this, type, oldBalance, Balance));
    }
}