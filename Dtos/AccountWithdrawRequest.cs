namespace BankingApi.Dtos;

using System.ComponentModel.DataAnnotations;

public class AccountWithdrawRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
}