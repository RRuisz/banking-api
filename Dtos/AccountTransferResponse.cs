namespace BankingApi.Dtos;

public class AccountTransferResponse
{
    public Guid AccountId { get; set; }
    public Guid TargetAccountId { get; set; }
    public decimal Amount { get; set; }
    public decimal NewBalance { get; set; }
}