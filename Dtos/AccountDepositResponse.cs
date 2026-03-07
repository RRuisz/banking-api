namespace BankingApi.Dtos;


public class AccountDepositResponse
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public decimal NewBalance { get; set; }
    public decimal OldBalance { get; set; }
}