namespace BankingApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using BankingApi.Dtos;
using BankingApi.Services;
using BankingApi.Exceptions;

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
    public async Task<IActionResult> GetAll([FromQuery] Guid ownerGuid)
    {
        var accounts = await _service.GetAll(ownerGuid);
        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AccountCreateRequest request)
    {
        try
        {
            var response = await _service.Create(request);
            return Ok(response);
        } catch (NotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }

    [HttpPost("{accountId}/deposit")]
    public async Task<IActionResult> Deposit(Guid accountId, AccountDepositRequest request)
    {
        try
        {
            var response = await _service.Deposit(accountId, request);
            return Ok(response);
        } catch(NotFoundException e)
        {
            return NotFound(new { message = e.Message });
        } catch(BadRequestException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("{accountId}/withdraw")]
    public async Task<IActionResult> Withdraw(Guid accountId, AccountWithdrawRequest request)
    {
        try
        {
            var response = await _service.Withdraw(accountId, request);
            return Ok(response);
        }
        catch (NotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (BadRequestException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet("{accountId}/statement")]
    public async Task<IActionResult> Statement(Guid accountId, [FromQuery] Guid ownerGuid)
    {
        try
        {
            var response = await _service.Statement(accountId, ownerGuid);
            return Ok(response);
        } catch (NotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }

    }

    [HttpGet("{accountId}/balance")]
    public async Task<IActionResult> GetBalance(Guid accountId, [FromQuery] Guid ownerGuid)
    {
        try
        {
            var response = await _service.GetBalance(accountId, ownerGuid);
            return Ok(response);
        } catch (NotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        
    }

    [HttpPost("{accountId}/transfer")]
    public async Task<IActionResult> Transfer(Guid accountId, AccountTransferRequest request)
    {

        try
        {
            var response = await _service.Transfer(accountId, request);
            return Ok(response);
        }
        catch (NotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (BadRequestException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }
    
}