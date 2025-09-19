using Microsoft.EntityFrameworkCore;
using PortalHelpdesk.Models;
using PortalHelpdesk.Models.Attachments;
using PortalHelpdesk.Models.Messages;

namespace PortalHelpdesk.Contexts
{
    public class HelpdeskContext(DbContextOptions<HelpdeskContext> options) : DbContext(options)
    {
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationMessage> ConversationMessages { get; set; }
        public DbSet<TicketPriority> TicketPriorities { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<TicketStatus> TicketStatus { get; set; }
        public DbSet<TicketHistory> TicketHistories { get; set; }
        public DbSet<Worklog> Worklogs { get; set; }
        public DbSet<TicketResolution> TicketResolutions { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<MessageAttachment> MessageAttachments { get; set; }
        public DbSet<ResolutionAttachment> ResolutionAttachments { get; set; }
        public DbSet<AtomicOperation> AtomicOperations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageAttachment>()
                .HasKey(ma => new { ma.MessageId, ma.AttachmentId });

            modelBuilder.Entity<ResolutionAttachment>()
                .HasKey(ra => new { ra.ResolutionId, ra.AttachmentId });

            modelBuilder.Entity<ConversationMessage>()
                .HasKey(cm => new { cm.ConversationId, cm.MessageId });

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Creator)
                .WithMany(u => u.TicketsCreated)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.TicketsAssigned)
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Requester)
                .WithMany(u => u.TicketsRequested)
                .HasForeignKey(t => t.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketHistory>()
                .HasOne(t => t.Modifier)
                .WithMany(u => u.TicketChanges)
                .HasForeignKey(t => t.ModifierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ResolutionAttachment>()
                .HasOne(ra => ra.TicketResolution)
                .WithMany(r => r.Attachments)
                .HasForeignKey(ra => ra.ResolutionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConversationMessage>()
                .HasOne(cm => cm.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(cm => cm.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageAttachment>()
                .HasOne(ma => ma.Message)
                .WithMany(m => m.Attachments)
                .HasForeignKey(ma => ma.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasData(
                new User { 
                    Id = 1, 
                    Name = "System",
                    Email = "",
                    CreatedAt = DateTime.UtcNow,
                    Role = "System",
                    IsActive = true
                }
                );

            modelBuilder.Entity<Group>().HasData(
                new Group { Id = 1, Name = "OP:Helpdesk", CreatedAt = DateTime.UtcNow },
                new Group { Id = 2, Name = "OP:Aquisição", CreatedAt = DateTime.UtcNow },
                new Group { Id = 3, Name = "OP:Infraestrutura", CreatedAt = DateTime.UtcNow },
                new Group { Id = 4, Name = "SAP:Contabilidade", CreatedAt = DateTime.UtcNow },
                new Group { Id = 5, Name = "SAP:Logística", CreatedAt = DateTime.UtcNow },
                new Group { Id = 6, Name = "SAP:Recursos Humanos", CreatedAt = DateTime.UtcNow },
                new Group { Id = 7, Name = "SSL:Smart Software Labs", CreatedAt = DateTime.UtcNow }
                );

            modelBuilder.Entity<TicketCategory>().HasData(
                new TicketCategory { Id = 1, Name = "Aquisição" },
                new TicketCategory { Id = 2, Name = "Cibersegurança" },
                new TicketCategory { Id = 3, Name = "Infor" },
                new TicketCategory { Id = 4, Name = "Microsoft 365" },
                new TicketCategory { Id = 5, Name = "Infraestrutura" },
                new TicketCategory { Id = 6, Name = "Portais e Websites" },
                new TicketCategory { Id = 7, Name = "Ferramentas Desktop" },
                new TicketCategory { Id = 8, Name = "SAP" },
                new TicketCategory { Id = 9, Name = "Contas e Acessos" },
                new TicketCategory { Id = 10, Name = "Outras" }
                );

            modelBuilder.Entity<TicketPriority>().HasData(
                new TicketPriority { Id = 1, Level = "Alta" },
                new TicketPriority { Id = 2, Level = "Média" },
                new TicketPriority { Id = 3, Level = "Baixa" },
                new TicketPriority { Id = 4, Level = "Normal" },
                new TicketPriority { Id = 5, Level = "Standard" }
                );

            modelBuilder.Entity<TicketStatus>().HasData(
                new TicketStatus { Id = 1, Status = "Aberto" },
                new TicketStatus { Id = 2, Status = "Em Andamento" },
                new TicketStatus { Id = 3, Status = "Resolvido" },
                new TicketStatus { Id = 4, Status = "Cancelado" },
                new TicketStatus { Id = 5, Status = "Encerrado" }
                );
        }

    }
}
