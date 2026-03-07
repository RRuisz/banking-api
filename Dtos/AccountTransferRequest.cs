namespace BankingApi.Dtos;

public class AccountTransferRequest
{
    public Guid OwnerGuid { get; set; }
    public Guid TargetAccountGuid { get; set; }
    public decimal Amount { get; set; }

}