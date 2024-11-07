using first_api_backend.Controllers;
using first_api_backend.Extensions;
using first_api_backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

var app = builder.Build();

app.ConfigureSwaggerExplorer()
    .ConfigureCORS(builder.Configuration)
    .AddIdentityAuthMiddlewares();

app.MapControllers();
app.MapGroup("/api")
    .MapIdentityApi<AppUser>();
app.MapGroup("/api")
    .MapIdentityUserEndpoints()
    .MapAccountEndpoints()
    .MapAuthorizationEndpoints();

app.Run();



