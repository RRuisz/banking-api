namespace BankingApi.Dtos;


public class CreateBankAccountResponse
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public Guid OwnerId { get; set; }
}