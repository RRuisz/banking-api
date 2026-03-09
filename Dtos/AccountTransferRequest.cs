namespace BankingApi.Dtos;

public class AccountTransferRequest
{
    public Guid TargetAccountGuid { get; set; }
    public decimal Amount { get; set; }
    public bool InstantTransfer { get; set; } = false;
}