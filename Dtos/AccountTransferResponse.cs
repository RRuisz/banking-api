namespace BankingApi.Dtos;

using BankingApi.Enums;

public class AccountTransferResponse
{
    public Guid AccountId { get; set; }
    public Guid TargetAccountId { get; set; }
    public decimal Amount { get; set; }
    public decimal NewBalance { get; set; }
    public decimal? Fee { get; set; }
    public TransactionStatus Status { get; set; }
}