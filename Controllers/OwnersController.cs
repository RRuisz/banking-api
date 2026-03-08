namespace BankingApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using BankingApi.Data;
using BankingApi.Dtos;
using BankingApi.Models;

[ApiController]
[Route("api/user")]
public class OwnersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OwnersController(AppDbContext context)
    {
        _context = context;
    }
    
}