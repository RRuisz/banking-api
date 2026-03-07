namespace BankingApi.Models;

public class BankAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Balance { get; private set; }

    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; } = null!;

    public List<Transaction> Transactions { get; } = [];

    private BankAccount() { }

    public BankAccount(Guid ownerId)
    {
        OwnerId = ownerId;
    }
}