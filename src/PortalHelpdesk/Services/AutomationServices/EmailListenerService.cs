using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Options;
using MimeKit;
using PortalHelpdesk.Configurations;
using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Messages;
using PortalHelpdesk.Services.DataPersistenceServices;

namespace PortalHelpdesk.Services.AutomationServices
{

    public class EmailListenerService : BackgroundService
    {
        private readonly ILogger<EmailListenerService> _logger;
        private readonly EmailConfiguration _emailConfig;
        private readonly UserDefaults _userDefaults;
        private TicketsService _ticketsService;
        private UsersService _usersService;
        private ConversationsService _conversationsService;
        private AttachmentsService _attachmentsService;

        private readonly IServiceScopeFactory _scopeFactory;

        public EmailListenerService(ILogger<EmailListenerService> logger, IOptions<EmailConfiguration> emailConfig, 
            IOptions<UserDefaults> userDefaults, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _emailConfig = emailConfig.Value;
            _userDefaults = userDefaults.Value;
            _scopeFactory = scopeFactory;
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
                    var client = await ConnectClient(stoppingToken);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

                    var messages = await FetchUnreadEmails(client, stoppingToken);

                    foreach (var item in messages)
                    {
                        if(await IsNewTicket(item))
                        
                            await CreateTicket(item);
                        else
                            await AppendMessageToConversation(item);
                        
                    }

                    await DisconnectClient(client, stoppingToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to load email messages: " + ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        public async Task<ImapClient> ConnectClient(CancellationToken stoppingToken)
        {
            var client = new ImapClient();
            await client.ConnectAsync(_emailConfig.ImapServer, _emailConfig.ImapPort, true, stoppingToken);
            await client.AuthenticateAsync(_emailConfig.Email, _emailConfig.Password, stoppingToken);

            return client;
        }

        public async Task DisconnectClient(ImapClient client, CancellationToken stoppingToken)
        {
            await client.DisconnectAsync(true, stoppingToken);
        }

        public async Task<List<MimeMessage>> FetchUnreadEmails(ImapClient client, CancellationToken stoppingToken)
        {
            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

            var results = await inbox.SearchAsync(SearchQuery.NotSeen, stoppingToken);
            List<MimeMessage> messages = new List<MimeMessage>();

            foreach (var uid in results)
            {
                var message = await inbox.GetMessageAsync(uid, stoppingToken);
                messages.Add(message);

                await MarkEmailAsRead(client, uid, stoppingToken);
            }
            return messages;
        }

        public async Task MarkEmailAsRead(ImapClient client, UniqueId uid, CancellationToken stoppingToken)
        {
            var inbox = client.Inbox;
            await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, stoppingToken);
        }
        public async Task<bool> IsNewTicket(MimeMessage message)
        {
            if (message.Subject.Contains("Re:", StringComparison.OrdinalIgnoreCase) ||
                message.Subject.Contains("Fwd:", StringComparison.OrdinalIgnoreCase))
            {
                return await FindTicket(message) == null;
            }

            return true;
        }

        public async Task<Ticket?> FindTicket(MimeMessage message)
        {
            var subjectParts = message.Subject?.Split(' ');
            var ticketPart = subjectParts?.FirstOrDefault(s => s != null && s.StartsWith('#') && s.Length > 0);
            string strTicketId = string.Empty;

            foreach (char c in ticketPart!)
            {
                if(char.IsDigit(c))
                    strTicketId += c;
            }

            if (ticketPart != null && int.TryParse(strTicketId, out int ticketId))
            {
                var existingTicket = await _ticketsService.GetTicketById(ticketId);
                return existingTicket;
            }

            return null;
        }

        public async Task CreateTicket(MimeMessage message)
        {
            var requester = await GetRequester(message);
            var creator = await _usersService.GetUserByEmail("SYSTEM\\system");

            var newMessage = new Message
            {
                From = message.From.ToString(),
                To = _emailConfig.Email,
                Cc = string.Join("; ", message.Cc.Mailboxes.Select(mb => mb.Address)),
                Subject = message.Subject,
                Content = EmailParser.ExtractHtmlContentWithInlineImages(message),
                MessageId = message.MessageId,
            };

            var ticket = new Ticket
            {
                RequesterId = requester?.Id,
                Requester = requester
            };

            await _ticketsService.CreateTicket(ticket, newMessage, creator!);
            await _attachmentsService.SaveEmailAttachments(newMessage, message);

            await Task.CompletedTask;
        }

        public async Task<User?> GetRequester(MimeMessage message)
        {
            var from = message.From.Mailboxes.First();
            var user = await _usersService.GetUserByEmail(from.Address);

            if (user == null)
            {
                user = new User
                {
                    Name = from.Name,
                    Email = from.Address,
                    IsActive = false,
                    Role = "Requester"
                };

                user = await _usersService.CreateUser(user);
            }

            return user;
        }

        public async Task AppendMessageToConversation(MimeMessage message)
        {
            var ticket = await FindTicket(message);
            if (ticket == null)
            {
                _logger.LogWarning("No ticket found for email with subject: {Subject}", message.Subject);
                return;
            }

            var newMessage = new Message
            {
                From = message.From.ToString(),
                To = _emailConfig.Email,
                Cc = string.Join("; ", message.Cc.Mailboxes.Select(mb => mb.Address)),
                Subject = message.Subject,
                Content = EmailParser.ExtractHtmlContentWithInlineImages(message),
                MessageId = message.MessageId,
                InReplyTo = message.InReplyTo,
                References = [.. message.References]
            };

            await _conversationsService.AddEmailMessageToConversation(newMessage, ticket);
            await _attachmentsService.SaveEmailAttachments(newMessage, message);

            await Task.CompletedTask;
        }
    }

}


