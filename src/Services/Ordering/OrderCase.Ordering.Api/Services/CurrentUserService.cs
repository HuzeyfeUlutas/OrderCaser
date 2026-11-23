using System.Security.Claims;
using OrderCase.Ordering.Api.Authentication;
using OrderCase.Ordering.Application.Abstractions.Services;

namespace OrderCase.Ordering.Api.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private const string SystemUser = "system";

    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string UserId =>
        User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? SystemUser;

    public bool IsAdmin =>
        User?.IsInRole(Roles.Admin) ?? false;
}