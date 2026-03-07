namespace BankingApi.Dtos;


public class AccountDepositRequest
{
    public Guid OwnerGuid { get; set; }
    public decimal Amount { get; set; }

}