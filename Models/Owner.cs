namespace BankingApi.Models;

public class Owner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public List<BankAccount> BankAccounts { get; set; } = [];
    
    public Owner() { }
    public Owner(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}