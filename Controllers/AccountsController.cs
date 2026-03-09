namespace BankingApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using BankingApi.Dtos;
using BankingApi.Services;
using BankingApi.Exceptions;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/bankaccounts")]
public class AccountsController : ControllerBase
{
    private readonly AccountService _service;
    
    public AccountsController(AccountService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var accounts = await _service.GetAll();
        return Ok(accounts);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var response = await _service.Create();
        return Ok(response);
    }

    [Authorize]
    [HttpPost("{accountId}/deposit")]
    public async Task<IActionResult> Deposit(Guid accountId, AccountDepositRequest request)
    {
        var response = await _service.Deposit(accountId, request);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("{accountId}/withdraw")]
    public async Task<IActionResult> Withdraw(Guid accountId, AccountWithdrawRequest request)
    { 
        var response = await _service.Withdraw(accountId, request);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{accountId}/statement")]
    public async Task<IActionResult> Statement(
        Guid accountId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await _service.Statement(accountId, page, pageSize);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{accountId}/balance")]
    public async Task<IActionResult> GetBalance(Guid accountId)
    {
        var response = await _service.GetBalance(accountId);

        return Ok(response);
    }

    [Authorize]
    [HttpPost("{accountId}/transfer")]
    public async Task<IActionResult> Transfer(Guid accountId, AccountTransferRequest request)
    {
        var response = await _service.Transfer(accountId, request);
        
        return Ok(response);
    }
    
}