using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using PortalHelpdesk.Configurations;
using PortalHelpdesk.Models;
using PortalHelpdesk.Services.DataPersistenceServices;
using DbMessage = PortalHelpdesk.Models.Messages.Message;
using DbUser = PortalHelpdesk.Models.User;

namespace PortalHelpdesk.Services.AutomationServices
{

    public class EmailListenerService : BackgroundService
    {
        private readonly ILogger<EmailListenerService> _logger;

        private readonly GraphServiceClient _graphClient;
        private readonly MicrosoftGraphConfig _config;

        private TicketsService _ticketsService;
        private UsersService _usersService;
        private ConversationsService _conversationsService;
        private AttachmentsService _attachmentsService;

        private readonly IServiceScopeFactory _scopeFactory;

        public EmailListenerService(ILogger<EmailListenerService> logger, GraphClientFactory factory, IOptions<MicrosoftGraphConfig> options,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _graphClient = factory.Create();
            _config = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                _ticketsService = scope.ServiceProvider.GetRequiredService<TicketsService>();
                _usersService = scope.ServiceProvider.GetRequiredService<UsersService>();
                _conversationsService = scope.ServiceProvider.GetRequiredService<ConversationsService>();
                _attachmentsService = scope.ServiceProvider.GetRequiredService<AttachmentsService>();

                try
                {
                    var messages = await FetchUnreadEmails();

                    foreach (var item in messages)
                    {
                        if (await IsNewTicket(item))

                            await CreateTicket(item);
                        else
                            await AppendMessageToConversation(item);

                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to load email messages: " + ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        public async Task<List<Message>> FetchUnreadEmails(int top = 10)
        {
            var result = await _graphClient.Users[_config.UserEmail]
                .MailFolders["Inbox"]
                .Messages
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = top;
                    requestConfiguration.QueryParameters.Orderby = ["receivedDateTime DESC"];
                    requestConfiguration.QueryParameters.Filter = "isRead eq false";
                    requestConfiguration.QueryParameters.Select =
                    [
                        "internetMessageHeaders",
                        "from",
                        "toRecipients",
                        "ccRecipients",
                        "subject",
                        "body",
                        "bodyPreview",
                        "receivedDateTime",
                        "importance",
                        "hasAttachments",
                        "conversationId",
                        "internetMessageId"
                    ];
                });

            var messages = result?.Value ?? [];
            // await MarkMessagesAsRead(messages);

            return messages;
        }

        public async Task MarkMessagesAsRead(List<Message> messages)
        {
            foreach (var message in messages)
            {
                await _graphClient.Users[_config.UserEmail]
                 .Messages[message.Id]
                 .PatchAsync(new Message { IsRead = true });
            }

        }
        public async Task<bool> IsNewTicket(Message message)
        {
            if (!string.IsNullOrEmpty(message.Subject) &&
                (message.Subject.Contains("Re:", StringComparison.OrdinalIgnoreCase) ||
                 message.Subject.Contains("Fwd:", StringComparison.OrdinalIgnoreCase)))
            {
                return await FindTicket(message) == null;
            }

            return true;
        }

        public async Task<Ticket?> FindTicket(Message message)
        {
            var subjectParts = message.Subject?.Split(' ');
            var ticketPart = subjectParts?.FirstOrDefault(s => s != null && s.StartsWith('#') && s.Length > 0);
            string strTicketId = string.Empty;

            foreach (char c in ticketPart!)
            {
                if (char.IsDigit(c))
                    strTicketId += c;
            }

            if (ticketPart != null && int.TryParse(strTicketId, out int ticketId))
            {
                var existingTicket = await _ticketsService.GetTicketById(ticketId);
                return existingTicket;
            }

            return null;
        }

        public async Task CreateTicket(Message message)
        {
            var requester = await GetRequester(message);
            var creator = await _usersService.GetSystemUser();

            var newMessage = new Models.Messages.Message
            {
                From = message.From?.EmailAddress?.Address!,
                To = _config.UserEmail,
                Cc = message.CcRecipients != null
                    ? string.Join("; ", message.CcRecipients.Select(mb => mb.EmailAddress?.Address))
                    : string.Empty,
                Subject = message.Subject ?? string.Empty,
                Content = await EmailParser.ExtractHtmlContentWithInlineImagesAsync(message, _graphClient),
                MessageId = message.InternetMessageId,
            };

            var ticket = new Ticket
            {
                RequesterId = requester?.Id,
                Requester = requester
            };

            await _ticketsService.CreateTicket(ticket, newMessage, creator!);
            await _attachmentsService.SaveEmailAttachments(newMessage, message, _graphClient);

            await Task.CompletedTask;
        }

        public async Task<DbUser?> GetRequester(Message message)
        {
            var fromEmail = message.From?.EmailAddress?.Address;
            var fromName = message.From?.EmailAddress?.Name;

            if (string.IsNullOrEmpty(fromEmail))
                return null;

            var user = await _usersService.GetUserByEmail(fromEmail);

            if (user == null)
            {
                user = new DbUser
                {
                    Name = fromName ?? fromEmail,
                    Email = fromEmail,
                    IsActive = false,
                    Role = "Requester"
                };

                user = await _usersService.CreateUser(user);
            }

            return user;
        }

        public async Task AppendMessageToConversation(Message message)
        {
            var ticket = await FindTicket(message);
            if (ticket == null)
            {
                _logger.LogWarning("No ticket found for email with subject: {Subject}", message.Subject);
                return;
            }

            var threadHeaders = GetThreadHeaders(message);

            var newMessage = new DbMessage
            {
                From = message.From?.EmailAddress?.Address ?? string.Empty,
                To = _config.UserEmail,
                Cc = message.CcRecipients != null
                    ? string.Join("; ", message.CcRecipients.Select(mb => mb.EmailAddress?.Address))
                    : string.Empty,
                Subject = message.Subject ?? string.Empty,
                Content = await EmailParser.ExtractHtmlContentWithInlineImagesAsync(message, _graphClient),
                MessageId = message.InternetMessageId,
                InReplyTo = threadHeaders.InReplyTo,
                References = [.. threadHeaders.References]
            };

            await _conversationsService.AddEmailMessageToConversation(newMessage, ticket);
            await _attachmentsService.SaveEmailAttachments(newMessage, message, _graphClient);

            await Task.CompletedTask;
        }

        public (string? InReplyTo, string[] References) GetThreadHeaders(Message message)
        {
            string? inReplyTo = message.InternetMessageHeaders?
                .FirstOrDefault(h => h?.Name != null && h.Name.Equals("In-Reply-To", System.StringComparison.OrdinalIgnoreCase))?.Value;

            string? strReferences = message.InternetMessageHeaders?
                .FirstOrDefault(h => h?.Name != null && h.Name.Equals("References", System.StringComparison.OrdinalIgnoreCase))?.Value;

            var references = strReferences?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [];

            return (inReplyTo, references);
        }
    }

}


