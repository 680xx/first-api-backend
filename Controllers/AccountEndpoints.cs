using System.Security.Claims;
using first_api_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace first_api_backend.Controllers;

public static class AccountEndpoints
{
    [Authorize]
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app) {
        app.MapGet("/UserProfile", GetUserProfile);
        return app;
    }


    private static async Task<IResult> GetUserProfile(
        ClaimsPrincipal user,
        UserManager<AppUser> userManager)
    {
        var userID = user.Claims.First(x => x.Type == "userID").Value;
        var userDetails = await userManager.FindByIdAsync(userID);
        return Results.Ok(
            new
            {
                Email = userDetails.Email,
                FullName = userDetails.FullName,
            }
            );
    }
}