namespace BankingApi.Dtos;

using BankingApi.Enums;

public class AccountStatementTransactionResponse
{
	public Guid Id { get; set; }
	public decimal Amount { get; set; }
	public TransactionType Type { get; set; }
	public decimal OldBalance { get; set; }
	public decimal NewBalance { get; set; }
	public DateTime Timestamp { get; set; }
}