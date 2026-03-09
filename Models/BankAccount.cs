namespace BankingApi.Models;

using BankingApi.Enums;

public class BankAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Balance { get; private set; }
    public decimal ReservedBalance { get; private set; } = 0;

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
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        Balance -= amount;
    }

    public decimal CalcFee(decimal amount)
    {
        return Math.Round(amount * 0.02m, 2);
    }

    public decimal CalcNewBalance(decimal amount, decimal fee)
    {
        return Balance - (amount + fee);
    }

    public void TransferSend(decimal amount)
    {
        Balance -= amount;
    }

    public void TransferReceive(decimal amount)
    {
        Balance += amount;
    }

    public void Reserve(decimal amount)
    {
        ReservedBalance += amount;
    }
    
    public void Unreserve(decimal amount)
    {
        ReservedBalance -= amount;
    }
}