namespace BankingApi.Dtos;

public class AccountStatementTransactionDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; }
}