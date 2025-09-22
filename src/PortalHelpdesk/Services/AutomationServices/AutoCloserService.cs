using PortalHelpdesk.Services.AutomationServices.Notifications;
using PortalHelpdesk.Services.DataPersistenceServices;

namespace PortalHelpdesk.Services.AutomationServices
{
    public class AutoCloserService : BackgroundService
    {
        private readonly ILogger<AutoCloserService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private TicketsService? _ticketsService;
        public AutoCloserService(ILogger<AutoCloserService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var scope = _scopeFactory.CreateScope();
                _ticketsService = scope.ServiceProvider.GetRequiredService<TicketsService>();

                _logger.LogInformation("AutoCloserService running at: {time}", DateTimeOffset.Now);

                // Check for tickets that have been resolved for more than 3 days and close them
                var threeDaysAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3));

                // Logic to close tickets goes here
                var ticketsToClose = await _ticketsService.GetTicketsResolvedBeforeAsync(threeDaysAgo);

                foreach (var ticket in ticketsToClose)
                {
                    await _ticketsService.CloseTicket(ticket);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
