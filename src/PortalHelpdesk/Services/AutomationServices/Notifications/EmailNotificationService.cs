using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PortalHelpdesk.Configurations;
using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Messages;
using PortalHelpdesk.Templates.EmailTemplateProvider;

namespace PortalHelpdesk.Services.AutomationServices.Notifications
{
    public class EmailNotificationService
    {
        private readonly EmailConfiguration _config;
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly IEmailTemplateProvider _emailTemplateProvider;

        public EmailNotificationService(IOptions<EmailConfiguration> config, ILogger<EmailNotificationService> logger,
            IEmailTemplateProvider emailTemplateProvider)
        {
            _config = config.Value;
            _logger = logger;
            _emailTemplateProvider = emailTemplateProvider;
        }

        public async Task SendTicketAcknowledgement(Ticket ticket)
        {
            if (ticket.Requester == null || ticket.Message == null)
            {
                _logger.LogWarning("Cannot send creation acknowledgement: " +
                    "Requester or ticket message is null. TicketId: {TicketId}", ticket.Id);
                return;
            }

            var subject = $"O seu pedido \"{ticket.Message.Subject}\" foi criado com ID: ##RE-{ticket.Id}##";
            var body = _emailTemplateProvider.GetTicketAcknowledgementTemplate(ticket);


            if (!string.IsNullOrWhiteSpace(ticket.Requester.Email))
            {
                var To = ticket.Requester.Email;
                var Cc = ticket.Message.Cc ?? "";

                var emailMessage = CreateMimeMessage(To, Cc, subject, body);
                await SendEmail(emailMessage);

            }
        }

        public async Task SendTicketRequesterChangeAcknowledgement(Ticket ticket)
        {
            if (ticket.Requester == null || ticket.Message == null)
            {
                _logger.LogWarning("Cannot send requester change acknowledgement: " +
                    "Requester or ticket message is null. TicketId: {TicketId}", ticket.Id);
                return;
            }

            var subject = $"O pedido com ID ##RE-{ticket.Id}## foi atribuído a si";
            var body = _emailTemplateProvider.GetTicketRequesterChangeTemplate(ticket);

            var To = ticket.Requester.Email;
            var Cc = ticket.Message.Cc ?? "";

            var emailMessage = CreateMimeMessage(To, Cc, subject, body);
            await SendEmail(emailMessage);
        }

        public async Task SendTicketAssignedAcknowledgement(Ticket ticket, User assignee)
        {

            if (ticket.Requester?.Email != null)
            {
                var requesterSubject = $"O seu pedido com ID ##RE-{ticket.Id}## foi atribuído a {assignee.Name}";
                var requesterBody = _emailTemplateProvider.GetTicketAssignedForRequesterTemplate(ticket, assignee);

                var To = ticket.Requester.Email;
                var Cc = ticket.Message?.Cc ?? "";

                var emailMessage = CreateMimeMessage(To, Cc, requesterSubject, requesterBody);
                await SendEmail(emailMessage);

            }
            else
            {
                _logger.LogWarning("Cannot send assignment notification to requester: " +
                    "Requester email is null. TicketId: {TicketId}", ticket.Id);
            }

            if (ticket.Message != null)
            {
                var assigneeSubject = $"O pedido com ID ##RE-{ticket.Id}## foi atribuído a si";
                var assigneeBody = _emailTemplateProvider.GetTicketAssignedForAssigneeTemplate(ticket);

                var To = assignee.Email;
                var Cc = ticket.Message?.Cc ?? "";

                var emailMessage = CreateMimeMessage(To, Cc, assigneeSubject, assigneeBody);
                await SendEmail(emailMessage);

            }
            else
            {
                _logger.LogWarning("Cannot send assignment notification to assignee: " +
                    "Ticket message is null. TicketId: {TicketId}", ticket.Id);
            }

        }

        public async Task SendTicketResolvedAcknowledgement(Ticket ticket, TicketResolution resolution)
        {
            if (ticket.Requester?.Email == null || ticket.Message?.Subject == null)
            {
                _logger.LogWarning("Cannot send resolved acknowledgement: " +
                    "Requester email or ticket subject is null. TicketId: {TicketId}", ticket.Id);
                return;
            }

            var subject = $"O seu pedido \"{ticket.Message.Subject}\" com ID ##RE-{ticket.Id}## foi resolvido";
            var body = _emailTemplateProvider.GetTicketResolvedTemplate(ticket, resolution);

            var To = ticket.Requester.Email;
            var Cc = ticket.Message.Cc ?? "";

            var emailMessage = CreateMimeMessage(To, Cc, subject, body);
            await SendEmail(emailMessage);
        }

        public async Task SendTicketCancelledAcknowledgement(Ticket ticket)
        {
            if (ticket.Requester?.Email == null || ticket.Message?.Subject == null)
            {
                _logger.LogWarning("Cannot send canceled acknowledgement: " +
                    "Requester email or ticket subject is null. TicketId: {TicketId}", ticket.Id);
                return;
            }
            var subject = $"O seu pedido \"{ticket.Message.Subject}\" com ID ##RE-{ticket.Id}## foi cancelado";
            var body = _emailTemplateProvider.GetTicketCancelledTemplate(ticket);

            var To = ticket.Requester.Email;
            var Cc = ticket.Message.Cc ?? "";

            var emailMessage = CreateMimeMessage(To, Cc, subject, body);
            await SendEmail(emailMessage);
        }

        public async Task SendTicketClosedAcknowledgement(Ticket ticket)
        {
            if (ticket.Requester?.Email == null || ticket.Message?.Subject == null)
            {
                _logger.LogWarning("Cannot send closed acknowledgement: " +
                    "Requester email or ticket subject is null. TicketId: {TicketId}", ticket.Id);
                return;
            }
            var subject = $"O seu pedido \"{ticket.Message.Subject}\" com ID ##RE-{ticket.Id}## foi encerrado";
            var body = _emailTemplateProvider.GetTicketClosedTemplate(ticket);

            var To = ticket.Requester.Email;
            var Cc = ticket.Message.Cc?? "";

            var emailMessage = CreateMimeMessage(To, Cc, subject, body);
            await SendEmail(emailMessage);
        }

        public async Task SendTicketReopenAcknowledgement(Ticket ticket)
        {
            if (ticket.Requester?.Email == null || ticket.Message?.Subject == null)
            {
                _logger.LogWarning("Cannot send reopen acknowledgement: " +
                    "Requester email or ticket subject is null. TicketId: {TicketId}", ticket.Id);
                return;
            }
            var subject = $"O seu pedido \"{ticket.Message.Subject}\" com ID ##RE-{ticket.Id}## foi reaberto";
            var body = _emailTemplateProvider.GetTicketReopenTemplate(ticket);

            var To = ticket.Requester.Email;
            var Cc = ticket.Message.Cc ?? "";

            var emailMessage = CreateMimeMessage(To, Cc, subject, body);
            await SendEmail(emailMessage);
        }

        public async Task SendNewMessageNotification(Ticket ticket)
        {
            if (ticket.Assignee?.Email == null)
            {
                _logger.LogWarning("Cannot send new message notification: " +
                    "Requester email, message subject or message content is null. TicketId: {TicketId}", ticket.Id);
                return;
            }
            var subject = $"Nova mensagem no pedido com ID ##RE-{ticket.Id}##";
            var body = _emailTemplateProvider.GetNewMessageTemplate(ticket);

            var To = ticket.Assignee.Email;
            var Cc = "";

            var emailMessage = CreateMimeMessage(To, Cc, subject, body);
            await SendEmail(emailMessage);
        }

        public async Task SendMessage(Ticket ticket, Message message)
        {
            if (ticket.Message == null || ticket.Requester?.Email == null || message.Subject == null || message.Content == null)
            {
                _logger.LogWarning("Cannot send message: " +
                    "Ticket subject, requester email, message subject or message content is null. TicketId: {TicketId}", ticket.Id);
                return;
            }
            var subject = $"Re: [Request ID: #{ticket.Id}] {ticket.Message.Subject}";
            var body = _emailTemplateProvider.GetMessageTemplate(message);

            var To = ticket.Requester.Email;
            var Cc = ticket.Message.Cc ?? "";

            var emailMessage = CreateMimeMessage(To, Cc, subject, body, message);
            await SendEmail(emailMessage);
        }

        private MimeMessage CreateMimeMessage(string to, string cc, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Helpdesk", _config.Email));
            message.To.Add(MailboxAddress.Parse(to));

            if (!string.IsNullOrWhiteSpace(cc))
            {
                foreach (var address in cc.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.Cc.Add(MailboxAddress.Parse(address.Trim()));
                }
            }

            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            return message;
        }
        private MimeMessage CreateMimeMessage(string to, string cc, string subject, string body, Message message)
        {
            var mimeMessage = CreateMimeMessage(to, cc, subject, body);
            mimeMessage.MessageId = message.MessageId;
            mimeMessage.InReplyTo = message.InReplyTo;
            if (message.References != null && message.References.Count > 0)
            {
                mimeMessage.References.AddRange(message.References);
            }

            return mimeMessage;
        }

        private async Task SendEmail(MimeMessage message)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(_config.SmtpServer, _config.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config.Email, _config.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }
}
