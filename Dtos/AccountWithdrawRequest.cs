namespace BankingApi.Dtos;


public class AccountWithdrawRequest
{
    public Guid OwnerGuid { get; set; }
    public decimal Amount { get; set; }
}