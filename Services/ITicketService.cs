using TicketQueryManagementSystem.Models;

namespace TicketQueryManagementSystem.Services
{
    public interface ITicketService
    {
        Task<Ticket> CreateTicketAsync(Ticket ticket);
        Task<Ticket> GetTicketByIdAsync(int id);
        Task<Ticket> GetTicketByNumberAsync(string ticketNumber);
        Task<List<Ticket>> GetAllTicketsAsync();
        Task<List<Ticket>> GetClientTicketsAsync(string clientId);
        Task<List<Ticket>> GetAssignedTicketsAsync(string developerId);
        Task<Ticket> UpdateTicketAsync(Ticket ticket);
        Task<bool> DeleteTicketAsync(int id);
        Task<List<Ticket>> GetTicketsByStatusAsync(TicketStatus status);
        Task<List<Ticket>> GetTicketsByPriorityAsync(int priorityId);
        Task<List<Ticket>> GetTicketsByCategoryAsync(int categoryId);
        Task<List<Ticket>> SearchTicketsAsync(string searchTerm);
        Task<bool> UpdateTicketStatusAsync(int ticketId, TicketStatus status);
        Task<bool> AssignTicketAsync(int ticketId, string developerId);
        Task<bool> CloseTicketAsync(int ticketId, string resolutionNotes);
        Task<TicketComment> AddCommentAsync(TicketComment comment);
        Task<List<TicketComment>> GetTicketCommentsAsync(int ticketId);
        Task<bool> DeleteCommentAsync(int commentId);
        Task<TicketAttachment> AddAttachmentAsync(TicketAttachment attachment);
        Task<List<TicketAttachment>> GetTicketAttachmentsAsync(int ticketId);
        Task<bool> DeleteAttachmentAsync(int attachmentId);
        Task<int> GetOpenTicketsCountAsync();
        Task<int> GetTicketsCountByStatusAsync(TicketStatus status);
        Task<Dictionary<string, int>> GetTicketStatisticsAsync();
    }
}