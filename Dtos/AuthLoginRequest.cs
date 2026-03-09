using System.ComponentModel.DataAnnotations;

namespace BankingApi.Dtos;

public class AuthLoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}