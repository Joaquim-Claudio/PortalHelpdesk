using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Contexts;
using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Messages;
using PortalHelpdesk.Services.AutomationServices.Notifications;

namespace PortalHelpdesk.Services.DataPersistenceServices
{
    public class ConversationsService
    {
        private readonly HelpdeskContext _context;
        private readonly EmailNotificationService _emailNotificationService;
        private readonly TicketsService _ticketsService;

        public ConversationsService(HelpdeskContext context, EmailNotificationService emailNotificationService,
            TicketsService ticketsService)
        {
            _context = context;
            _emailNotificationService = emailNotificationService;
            _ticketsService = ticketsService;
        }

        public async Task<Conversation?> GetConversationByTicketId(Ticket ticket)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Messages!)
                    .ThenInclude(cm => cm.Message.Attachments!)
                        .ThenInclude(ma => ma.Attachment)
                .FirstOrDefaultAsync(c => c.TicketId == ticket.Id);

            return conversation;
        }

        public async Task<Conversation?> AddEmailMessageToConversation(Message newMessage, Ticket ticket)
        {

            var conversation = await GetOrCreateConversation(ticket);

            newMessage.SentAt = DateTime.UtcNow;
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            var newConversationMessage = new ConversationMessage
            {
                Conversation = conversation!,
                Message = newMessage
            };

            conversation?.Messages?.Add(newConversationMessage);
            await _context.SaveChangesAsync();

            await _emailNotificationService.SendNewMessageNotification(ticket);

            if (_ticketsService.IsTicketResolved(ticket))
            {
                await _ticketsService.ReopenTicket(ticket);
            }

            return conversation;
        }

        public async Task<Conversation?> AddMessageToConversation(Message newMessage, Ticket ticket)
        {
            if( ticket.Message != null)
                ticket.Message.Cc = newMessage.Cc;

            var conversation = await GetOrCreateConversation(ticket);
            var lastMessage = conversation?.Messages?
                .OrderByDescending(cm => cm.Message.SentAt)
                .FirstOrDefault()?.Message;

            lastMessage ??= ticket.Message;

            newMessage.SentAt = DateTime.UtcNow;
            newMessage.MessageId = MimeKit.Utils.MimeUtils.GenerateMessageId("helpdesk.cosmuz.com");
            newMessage.InReplyTo = lastMessage?.MessageId;
            newMessage.References ??= [];

            if (lastMessage != null)
            {
                if (!string.IsNullOrEmpty(lastMessage.MessageId))
                    newMessage.References.Add(lastMessage.MessageId);

                foreach (var reference in lastMessage.References ?? [])
                    newMessage.References.Add(reference);
            }

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            var newConversationMessage = new ConversationMessage
            {
                Conversation = conversation!,
                Message = newMessage
            };

            conversation?.Messages?.Add(newConversationMessage);
            await _context.SaveChangesAsync();

            await _emailNotificationService.SendMessage(ticket, newMessage);

            return conversation;
        }

        public async Task<Conversation?> GetOrCreateConversation(Ticket ticket)
        {
            var existingConversation = await GetConversationByTicketId(ticket);

            if (existingConversation != null)
            {
                return existingConversation;
            }

            var conversation = new Conversation
            {
                TicketId = ticket.Id,
                Messages = [],
                CreatedAt = DateTime.UtcNow
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            return conversation;
        }

        public async Task<Message?> GetMessageById(int messageId)
        {
            var message = await _context.Messages
                .Include(c => c.Attachments)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            return message;
        }
    }
}
