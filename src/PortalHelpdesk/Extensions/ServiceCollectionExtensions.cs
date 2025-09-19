using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using PortalHelpdesk.Configurations;
using PortalHelpdesk.Filters;
using PortalHelpdesk.Services.AutomationServices;
using PortalHelpdesk.Services.AutomationServices.Notifications;
using PortalHelpdesk.Services.DataPersistenceServices;
using PortalHelpdesk.Templates.EmailTemplateProvider;

namespace PortalHelpdesk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfigurations(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<FileUploadOptions>(config.GetSection("FileUpload"));
            services.Configure<EmailConfiguration>(config.GetSection("EmailConfiguration"));
            services.Configure<UserDefaults>(config.GetSection("UserDefaults"));
            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<PorpertiesService>();
            services.AddScoped<TicketsService>();
            services.AddScoped<UsersService>();
            services.AddScoped<WorklogsService>();
            services.AddScoped<GroupsService>();
            services.AddScoped<ConversationsService>();
            services.AddScoped<AttachmentsService>();
            services.AddScoped<TicketHistoryService>();
            services.AddScoped<TicketStatusService>();
            services.AddScoped<TicketResolutionsService>();
            services.AddScoped<EmailNotificationService>();
            services.AddScoped<IEmailTemplateProvider, EmailTemplateProvider>();

            services.AddHostedService<EmailListenerService>();
            services.AddHostedService<AutoCloserService>();

            services.AddScoped<UserResolverFilter>();

            return services;
        }

        public static IServiceCollection AddAppDatabase(this IServiceCollection services, IConfiguration config)
        {
            var connString = config.GetConnectionString("DefaultConnection") ?? "";
            services.AddDbContext<PortalHelpdesk.Contexts.HelpdeskContext>(
                opt => opt.UseNpgsql(connString)
            );
            return services;
        }

        public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration config)
        {
            var corsOrigins = config.GetValue<string>("CorsAllowedOrigins")?.Split(',') ?? [];
            const string policyName = "_AllowSpecificOrigins";

            services.AddCors(options =>
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.WithOrigins(corsOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(config.GetSection("AzureAd"));

            return services;
        }
    }

}

