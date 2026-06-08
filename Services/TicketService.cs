using Microsoft.EntityFrameworkCore;
using TicketQueryManagementSystem.Data;
using TicketQueryManagementSystem.Models;

namespace TicketQueryManagementSystem.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TicketService> _logger;

        public TicketService(ApplicationDbContext context, ILogger<TicketService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            ticket.TicketNumber = await GenerateTicketNumberAsync();
            ticket.CreatedAt = DateTime.UtcNow;
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket> GetTicketByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Comments).ThenInclude(c => c.User)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Ticket> GetTicketByNumberAsync(string ticketNumber)
        {
            return await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetClientTicketsAsync(string clientId)
        {
            return await _context.Tickets
                .Where(t => t.ClientId == clientId)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetAssignedTicketsAsync(string developerId)
        {
            return await _context.Tickets
                .Where(t => t.AssignedToId == developerId)
                .Include(t => t.Client)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Ticket> UpdateTicketAsync(Ticket ticket)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return false;
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Ticket>> GetTicketsByStatusAsync(TicketStatus status)
        {
            return await _context.Tickets
                .Where(t => t.Status == status)
                .Include(t => t.Client)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetTicketsByPriorityAsync(int priorityId)
        {
            return await _context.Tickets
                .Where(t => t.PriorityId == priorityId)
                .Include(t => t.Client)
                .Include(t => t.Category)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetTicketsByCategoryAsync(int categoryId)
        {
            return await _context.Tickets
                .Where(t => t.CategoryId == categoryId)
                .Include(t => t.Client)
                .Include(t => t.Priority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Ticket>> SearchTicketsAsync(string searchTerm)
        {
            return await _context.Tickets
                .Where(t => t.Title.Contains(searchTerm) || t.Description.Contains(searchTerm) || t.TicketNumber.Contains(searchTerm))
                .Include(t => t.Client)
                .Include(t => t.Category)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateTicketStatusAsync(int ticketId, TicketStatus status)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return false;
            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;
            if (status == TicketStatus.Closed) ticket.ClosedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignTicketAsync(int ticketId, string developerId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return false;
            ticket.AssignedToId = developerId;
            ticket.Status = TicketStatus.InProgress;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CloseTicketAsync(int ticketId, string resolutionNotes)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return false;
            ticket.Status = TicketStatus.Closed;
            ticket.ClosedAt = DateTime.UtcNow;
            ticket.IsResolved = true;
            ticket.ResolutionNotes = resolutionNotes;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TicketComment> AddCommentAsync(TicketComment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            _context.TicketComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<List<TicketComment>> GetTicketCommentsAsync(int ticketId)
        {
            return await _context.TicketComments
                .Where(c => c.TicketId == ticketId)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var comment = await _context.TicketComments.FindAsync(commentId);
            if (comment == null) return false;
            _context.TicketComments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TicketAttachment> AddAttachmentAsync(TicketAttachment attachment)
        {
            attachment.UploadedAt = DateTime.UtcNow;
            _context.TicketAttachments.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<List<TicketAttachment>> GetTicketAttachmentsAsync(int ticketId)
        {
            return await _context.TicketAttachments
                .Where(a => a.TicketId == ticketId)
                .Include(a => a.UploadedBy)
                .OrderByDescending(a => a.UploadedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteAttachmentAsync(int attachmentId)
        {
            var attachment = await _context.TicketAttachments.FindAsync(attachmentId);
            if (attachment == null) return false;
            _context.TicketAttachments.Remove(attachment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetOpenTicketsCountAsync()
        {
            return await _context.Tickets.CountAsync(t => t.Status != TicketStatus.Closed);
        }

        public async Task<int> GetTicketsCountByStatusAsync(TicketStatus status)
        {
            return await _context.Tickets.CountAsync(t => t.Status == status);
        }

        public async Task<Dictionary<string, int>> GetTicketStatisticsAsync()
        {
            return new Dictionary<string, int>
            {
                { "Total", await _context.Tickets.CountAsync() },
                { "Open", await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Open) },
                { "InProgress", await _context.Tickets.CountAsync(t => t.Status == TicketStatus.InProgress) },
                { "OnHold", await _context.Tickets.CountAsync(t => t.Status == TicketStatus.OnHold) },
                { "Resolved", await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Resolved) },
                { "Closed", await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Closed) }
            };
        }

        private async Task<string> GenerateTicketNumberAsync()
        {
            var lastTicket = await _context.Tickets.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            int nextNumber = (lastTicket?.Id ?? 0) + 1;
            return $"TKT-{nextNumber:D6}";
        }
    }
}