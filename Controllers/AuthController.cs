namespace BankingApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using BankingApi.Services;
using BankingApi.Dtos;
using BankingApi.Exceptions;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthRegisterRequest request)
    {
        var response = await _service.Register(request);

        return Ok(response);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthLoginRequest request)
    {
        var response = await _service.Login(request);

        return Ok(response);
    }
}