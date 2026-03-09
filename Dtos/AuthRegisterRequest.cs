using System.ComponentModel.DataAnnotations;

namespace BankingApi.Dtos;

public class AuthRegisterRequest
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
}