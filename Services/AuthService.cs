namespace BankingApi.Services;

using BankingApi.Data;
using BankingApi.Models;
using BankingApi.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankingApi.Exceptions;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<Owner> _passwordHasher = new();

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> Register(AuthRegisterRequest request)
    {
        var exists = await _context.Owners.AnyAsync(o => o.Email == request.Email);
        
        if (exists)
            throw new InvalidOperationsException("Email already exists");

        var owner = new Owner
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
        };

        owner.PasswordHash = _passwordHasher.HashPassword(owner, request.Password);
        
        await _context.Owners.AddAsync(owner);
        await _context.SaveChangesAsync();
        
        return GenerateToken(owner);
    }

    public async Task<AuthResponse> Login(AuthLoginRequest request)
    {
        var owner = await _context.Owners.SingleOrDefaultAsync(o => o.Email == request.Email);
        if (owner == null)
        {
            throw new InvalidOperationsException("Invalid Credentials");
        }

        var result = _passwordHasher.VerifyHashedPassword(owner, owner.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException("Email already exists");
        }
        
        return GenerateToken(owner);
    }

    private AuthResponse GenerateToken(Owner owner)
    {
        var key = _configuration["JWT:Key"]!;
        var issuer = _configuration["JWT:Issuer"]!;
        var audience = _configuration["JWT:Audience"]!;
        var expiryMinutes = int.Parse(_configuration["JWT:ExpiryMinutes"]!);

        var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, owner.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, owner.Email),
        new(ClaimTypes.NameIdentifier, owner.Id.ToString()),
        new(ClaimTypes.Name, owner.Email)
    };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expires
        };
    }

}