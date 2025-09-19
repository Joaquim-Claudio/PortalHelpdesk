using Serilog;

namespace PortalHelpdesk.Extensions
{
    public static class LoggingExtensions
    {
        public static void AddAppLogging(this ConfigureHostBuilder host, IConfiguration config)
        {
            var connString = config.GetConnectionString("DefaultConnection") ?? "";
            host.UseSerilog((context, services, configuration) =>
                configuration
                    .WriteTo.Console()
                    .WriteTo.PostgreSQL(
                        connectionString: connString,
                        tableName: "Logs",
                        needAutoCreateTable: true
                    )
            );
        }
    }
}
