using System.Security.Claims;
using CodeExecutor.Common.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace CodeExecutor.Common.Security;

public static class AuthenticationExtensions
{
    public static bool TryParseUser(this HttpContext context, out AppUser user)
    {
        var usernameClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        var isSuperUserClaim = context.User.Claims.FirstOrDefault(c => c.Type == "IsSuperUser");

        if (usernameClaim is null || 
            !bool.TryParse(isSuperUserClaim?.Value.ToLower(), out var isSuper) ||
            !long.TryParse(userIdClaim?.Value, out var userId))
        {
            user = new AppUser();
            return false;
        }
        
        user = new AppUser
        {
            Id = userId,
            Username = usernameClaim.Value,
            IsSuperUser = isSuper,
        };
        return true;
    }
}