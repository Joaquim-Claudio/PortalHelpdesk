using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Configurations;
using PortalHelpdesk.Services;
using PortalHelpdesk.Services.AutomationServices;
using PortalHelpdesk.Services.AutomationServices.Notifications;
using PortalHelpdesk.Services.DataPersistenceServices;
using PortalHelpdesk.Templates.EmailTemplateProvider;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FileUploadOptions>(
    builder.Configuration.GetSection("FileUpload"));

builder.Services.Configure<EmailConfiguration>(
    builder.Configuration.GetSection("EmailConfiguration"));

builder.Services.Configure<UserDefaults>(
    builder.Configuration.GetSection("UserDefaults"));

// Add services to the container.
builder.Services.AddScoped<PorpertiesService>();
builder.Services.AddScoped<TicketsService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<WorklogsService>();
builder.Services.AddScoped<GroupsService>();
builder.Services.AddScoped<ConversationsService>();
builder.Services.AddScoped<AttachmentsService>();
builder.Services.AddScoped<TicketHistoryService>();
builder.Services.AddScoped<TicketStatusService>();
builder.Services.AddScoped<TicketResolutionsService>();
builder.Services.AddScoped<EmailNotificationService>();
builder.Services.AddScoped<IEmailTemplateProvider, EmailTemplateProvider>();

builder.Services.AddScoped<UserResolverFilter>();

builder.Services.AddHostedService<EmailListenerService>();
builder.Services.AddHostedService<AutoCloserService>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<UserResolverFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

string connString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

builder.Services.AddDbContext<PortalHelpdesk.Contexts.HelpdeskContext>(opt =>
    opt.UseNpgsql(connString)
    );

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .WriteTo.Console()
        .WriteTo.PostgreSQL(
            connectionString: connString,
            tableName: "Logs",
            needAutoCreateTable: true
        )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
