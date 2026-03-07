namespace BankingApi.Dtos;


public class AccountWithdrawResponse
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public decimal Amount { get; set; }
    public decimal OldBalance { get; set; }
    public decimal NewBalance { get; set; }
}