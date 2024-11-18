using Microsoft.AspNetCore.Authorization;

namespace first_api_backend.Controllers;

public static class AuthorizationDemoEndpoints
{
    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/AdminOnly", AdminOnly);
        
        app.MapGet("/AdminOrOwner", [Authorize(Roles = "Admin, Owner")] () =>
            { return "Admin Or Owner"; });
        
        app.MapGet("/LibraryMembersOnly", [Authorize(Policy = "HasLibraryId")] () => 
            { return "Library members only"; });
        
        app.MapGet("/FemaleUsersOnly", [Authorize(Roles = "User", Policy = "FemalesOnly")] () => 
            { return "Female users only"; });
        
        app.MapGet("/MaleUsersUnderAgeOf10Only", [Authorize(Roles = "User", Policy = "MaleOnly")] [Authorize(Policy = "Under10")] () => 
            { return "Male users under the age of 10 only"; });
        
        return app;
    }

    [Authorize(Roles = "Admin")]
    private static string AdminOnly()
    {
        return "Admin only";
    }
}