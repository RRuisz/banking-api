namespace BankingApi.Dtos;


public class AccountCreateResponse
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public Guid OwnerId { get; set; }
}