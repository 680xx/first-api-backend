using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using first_api_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace first_api_backend.Controllers
{

    public class UserRegistrationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int? LibraryID { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public static class IdentityUserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/signup", CreateUser);
            app.MapPost("/signin", SignIn);
            app.MapPut("/updateuser/{userId}", UpdateUser);
            app.MapDelete("/deleteuser/{userId}", DeleteUser);
            app.MapGet("/users", GetAllUsers);
            return app;
        }
        
        [AllowAnonymous]
        private static async Task<IResult> CreateUser(UserManager<AppUser> userManager,
            [FromBody] UserRegistrationModel userRegistrationModel)
        {
            AppUser user = new AppUser()
            {
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName,
                Gender = userRegistrationModel.Gender,
                DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-userRegistrationModel.Age)),
                LibararyID = userRegistrationModel.LibraryID,
            };
            var result = await userManager.CreateAsync(
                user,
                userRegistrationModel.Password);
            await userManager.AddToRoleAsync(user, userRegistrationModel.Role);

            if (result.Succeeded)
                return Results.Ok(result);
            
            else
                return Results.BadRequest(result);
        }
        
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public static async Task<IResult> GetAllUsers(UserManager<AppUser> userManager)
        {
            // Hämta alla användare
            var users = userManager.Users.ToList(); 

            if (users == null || users.Count == 0)
            {
                return Results.NotFound(new { message = "No users found" });
            }

            var userModels = users.Select(user => new 
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.Gender,
                Age = DateTime.Now.Year - user.DOB.Year,  // Beräkna ålder från födelsedatum
                user.LibararyID
            }).ToList();

            return Results.Ok(userModels);
        }

        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public static async Task<IResult> UpdateUser(UserManager<AppUser> userManager,
            [FromBody] UserRegistrationModel userRegistrationModel, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Results.NotFound(new { message = "User not found" });
            }

            user.FullName = userRegistrationModel.FullName ?? user.FullName;
            user.Gender = userRegistrationModel.Gender ?? user.Gender;
            user.DOB = userRegistrationModel.Age != null ? DateOnly.FromDateTime(DateTime.Now.AddYears(-userRegistrationModel.Age)) : user.DOB;
            user.LibararyID = userRegistrationModel.LibraryID ?? user.LibararyID;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Results.Ok(user);
            else
                return Results.BadRequest(result);
        }
        
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public static async Task<IResult> DeleteUser(UserManager<AppUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Results.NotFound(new { message = "User not found" });
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
                return Results.Ok(new { message = "User deleted successfully" });
            else
                return Results.BadRequest(result);
        }
        
        [AllowAnonymous]
        private static async Task<IResult> SignIn(UserManager<AppUser> userManager,
            [FromBody] LoginModel loginModel,
            IOptions<AppSettings> appSettings)
        {
            var user = await userManager.FindByEmailAsync(loginModel.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var roles = await userManager.GetRolesAsync(user);
                var signInKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret)
                );
                ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
                {
                    new Claim("userID", user.Id.ToString()),
                    new Claim("gender", user.Gender.ToString()),
                    new Claim("age", (DateTime.Now.Year - user.DOB.Year).ToString()),
                    new Claim(ClaimTypes.Role,roles.First()),
                });
                
                if (user.LibararyID != null)
                    claims.AddClaim(new Claim("libraryID", user.LibararyID.ToString()!));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    // Expires = DateTime.UtcNow.AddMinutes(1),
                    Expires = DateTime.UtcNow.AddDays(10),
                    SigningCredentials = new SigningCredentials(
                        signInKey,
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Results.Ok(new { token });
            }
            else
            {
                return Results.BadRequest(new { message = "Username or password is incorrect." });
            }
        }
    }
}