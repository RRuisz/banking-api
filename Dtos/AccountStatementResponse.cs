namespace BankingApi.Dtos;

using BankingApi.Models;

public class AccountStatementResponse
{
    public Guid AccountId { get; set; }
    public Guid OwnerId { get; set; }
    public List<AccountStatementTransactionResponse> Transactions { get; set; } = [];
}
