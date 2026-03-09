using System.ComponentModel.DataAnnotations;

namespace BankingApi.Dtos;

public class AccountTransferRequest
{
    [Required]
    public Guid? TargetAccountGuid { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
    public bool InstantTransfer { get; set; } = false;
}