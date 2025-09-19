using DotNetEnv;
using PortalHelpdesk.Extensions;
using PortalHelpdesk.Filters;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Dependency Injection - DI
builder.Services.AddAppConfigurations(builder.Configuration);
builder.Services.AddAppServices();
builder.Services.AddAppDatabase(builder.Configuration);
builder.Services.AddAppCors(builder.Configuration);
builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<UserResolverFilter>();
});

builder.Host.AddAppLogging(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("_AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
