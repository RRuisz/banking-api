namespace BankingApi.Dtos;

public class AccountGetBalanceResponse
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
}