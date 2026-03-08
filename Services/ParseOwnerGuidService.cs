namespace BankingApi.Services;

using System.Security.Claims;
public class ParseOwnerGuidService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ParseOwnerGuidService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        
        return Guid.Parse(claim!);
    }
}