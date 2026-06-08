using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketQueryManagementSystem.Models;

namespace TicketQueryManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Priority> Priorities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Ticket>()
                .HasOne(t => t.Client)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(t => t.AssignedTo)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Ticket>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tickets)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(t => t.Priority)
                .WithMany(p => p.Tickets)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketComment>()
                .HasOne(tc => tc.Ticket)
                .WithMany(t => t.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TicketComment>()
                .HasOne(tc => tc.User)
                .WithMany(u => u.Comments)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketAttachment>()
                .HasOne(ta => ta.Ticket)
                .WithMany(t => t.Attachments)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TicketAttachment>()
                .HasOne(ta => ta.UploadedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            builder.Entity<Priority>().HasData(
                new Priority { Id = 1, Level = PriorityLevel.Low, Description = "Low Priority", ColorCode = "#28a745" },
                new Priority { Id = 2, Level = PriorityLevel.Medium, Description = "Medium Priority", ColorCode = "#ffc107" },
                new Priority { Id = 3, Level = PriorityLevel.High, Description = "High Priority", ColorCode = "#fd7e14" },
                new Priority { Id = 4, Level = PriorityLevel.Critical, Description = "Critical Priority", ColorCode = "#dc3545" }
            );

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Type = CategoryType.Bug, Name = "Bug", Description = "Report a bug or defect" },
                new Category { Id = 2, Type = CategoryType.Enhancement, Name = "Enhancement", Description = "Suggest an improvement" },
                new Category { Id = 3, Type = CategoryType.FeatureRequest, Name = "Feature Request", Description = "Request a new feature" },
                new Category { Id = 4, Type = CategoryType.Support, Name = "Support", Description = "Get technical support" },
                new Category { Id = 5, Type = CategoryType.Query, Name = "Query", Description = "General inquiry" }
            );
        }
    }
}