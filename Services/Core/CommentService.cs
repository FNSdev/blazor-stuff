using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hephaestus.Models;

namespace hephaestus.Services
{
    public class CommentService 
    {
        private DatabaseContext _databaseContext;
        private ToastService _toastService;

        public CommentService(DatabaseContext databaseContext, ToastService toastService)
        {
            _databaseContext = databaseContext;
            _toastService = toastService;
        }

        public async Task<bool> CreateComment(Ticket ticket, User user, string message)
        {
            var comment = new Comment
            {
                Ticket = ticket,
                User = user,
                Message = message,
                CreatedAt = DateTime.UtcNow,
            };
            await _databaseContext.Comments.AddAsync(comment);
            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Comment>> FindByTicket(Ticket ticket)
        {
            return await _databaseContext.Comments
                .Where(c => c.TicketId == ticket.Id)
                .Select(c => c)
                .ToListAsync();
        }
    }
}
