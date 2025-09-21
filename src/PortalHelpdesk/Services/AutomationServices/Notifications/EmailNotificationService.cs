using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using PortalHelpdesk.Configurations;
using PortalHelpdesk.Models;
using PortalHelpdesk.Templates.EmailTemplateProvider;
using DbMessage = PortalHelpdesk.Models.Messages.Message;
using DbUser = PortalHelpdesk.Models.User;

namespace PortalHelpdesk.Services.AutomationServices.Notifications
{
    public class EmailNotificationService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly MicrosoftGraphConfig _msGraphConfig;
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly IEmailTemplateProvider _emailTemplateProvider;

        public EmailNotificationService(GraphClientFactory factory, IOptions<MicrosoftGraphConfig> msGraphConfig,
            ILogger<EmailNotificationService> logger, IEmailTemplateProvider emailTemplateProvider)
        {
            _graphClient = factory.Create();
            _msGraphConfig = msGraphConfig.Value;
            _logger = logger;
            _emailTemplateProvider = emailTemplateProvider;
        }

        private Message BuildGraphMessage(string subject, string body, string to, string? cc = null,
            string? messageId = null, string? inReplyto = null, List<string>? references = null)
        {
            var message = new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = body
                },
                BodyPreview = body.Length > 255 ? string.Concat(body.AsSpan(0, 252), "...") : body,
                From = new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Name = "Helpdesk",
                        Address = _msGraphConfig.UserEmail
                    }
                },
                ToRecipients =
                [
                    new() { EmailAddress = new EmailAddress { Address = to } }
                ],
                CcRecipients = cc?.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(e => new Recipient { EmailAddress = new EmailAddress { Address = e.Trim() } })
                                  .ToList(),
                InternetMessageId = messageId?.Trim()
            };

            if (!string.IsNullOrEmpty(inReplyto))
            {
                message.InternetMessageHeaders ??= [];
                message.InternetMessageHeaders.Add(new InternetMessageHeader
                {
                    Name = "In-Reply-to",
                    Value = inReplyto
                });
            }

            if (references != null && references.Count > 0)
            {
                message.InternetMessageHeaders ??= [];
                message.InternetMessageHeaders.Add(new InternetMessageHeader
                {
                    Name = "References",
                    Value = string.Join(" ", references)
                });
            }

            return message;
        }

        private async Task SendGraphMessageAsync(Message message)
        {
            try
            {
                //await _graphClient.Users[_msGraphConfig.UserEmail]
                //    .SendMail
                //    .PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
                //    {
                //        Message = message,
                //        SaveToSentItems = true
                //    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via Graph API.");
            }
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
                var to = ticket.Requester.Email;
                var cc = ticket.Message.Cc ?? "";

                var emailMessage = BuildGraphMessage(subject, body, to, cc);
                await SendGraphMessageAsync(emailMessage);

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

            var to = ticket.Requester.Email;
            var cc = ticket.Message.Cc ?? "";

            var emailMessage = BuildGraphMessage(subject, body, to, cc);
            await SendGraphMessageAsync(emailMessage);
        }

        public async Task SendTicketAssignedAcknowledgement(Ticket ticket, DbUser assignee)
        {

            if (ticket.Requester?.Email != null)
            {
                var requesterSubject = $"O seu pedido com ID ##RE-{ticket.Id}## foi atribuído a {assignee.Name}";
                var requesterBody = _emailTemplateProvider.GetTicketAssignedForRequesterTemplate(ticket, assignee);

                var to = ticket.Requester.Email;
                var cc = ticket.Message?.Cc ?? "";

                var emailMessage = BuildGraphMessage(requesterSubject, requesterBody, to, cc);
                await SendGraphMessageAsync(emailMessage);

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

                var to = assignee.Email;
                var cc = ticket.Message?.Cc ?? "";

                var emailMessage = BuildGraphMessage(assigneeSubject, assigneeBody, to, cc);
                await SendGraphMessageAsync(emailMessage);

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

            var to = ticket.Requester.Email;
            var cc = ticket.Message.Cc ?? "";

            var emailMessage = BuildGraphMessage(subject, body, to, cc);
            await SendGraphMessageAsync(emailMessage);
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

            var to = ticket.Requester.Email;
            var cc = ticket.Message.Cc ?? "";

            var emailMessage = BuildGraphMessage(subject, body, to, cc);
            await SendGraphMessageAsync(emailMessage);
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

            var to = ticket.Requester.Email;
            var cc = ticket.Message.Cc ?? "";

            var emailMessage = BuildGraphMessage(subject, body, to, cc);
            await SendGraphMessageAsync(emailMessage);
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

            var to = ticket.Requester.Email;
            var cc = ticket.Message.Cc ?? "";

            var emailMessage = BuildGraphMessage(subject, body, to, cc);
            await SendGraphMessageAsync(emailMessage);
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

            var to = ticket.Assignee.Email;
            var cc = "";

            var emailMessage = BuildGraphMessage(subject, body, to, cc);
            await SendGraphMessageAsync(emailMessage);
        }

        public async Task SendMessage(Ticket ticket, DbMessage message)
        {
            if (ticket.Message == null || ticket.Requester?.Email == null || message.Subject == null || message.Content == null)
            {
                _logger.LogWarning("Cannot send message: " +
                    "Ticket subject, requester email, message subject or message content is null. TicketId: {TicketId}", ticket.Id);
                return;
            }
            var subject = $"Re: [Request ID: #{ticket.Id}] {ticket.Message.Subject}";
            var body = _emailTemplateProvider.GetMessageTemplate(message);

            var to = ticket.Requester.Email;
            var cc = ticket.Message.Cc ?? "";

            var emailMessage = BuildGraphMessage(subject, body, to, cc, message.MessageId, message.InReplyTo, message.References);
            await SendGraphMessageAsync(emailMessage);
        }
    }
}
