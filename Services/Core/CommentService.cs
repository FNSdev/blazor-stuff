using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using hephaestus.Models;

namespace hephaestus.Services
{
    public class CommentService 
    {
        private DatabaseContext _databaseContext;
        private ToastService _toastService;
        private MailingService _mailingService;
        public User ServiceBot { get; }

        public CommentService(DatabaseContext databaseContext, ToastService toastService, MailingService mailingService)
        {
            _databaseContext = databaseContext;
            _toastService = toastService;
            _mailingService = mailingService;
            ServiceBot = databaseContext.Users
                .Where(u => u.UserName == "ServiceBot")
                .Select(u => u)
                .Single();
        }

        public async Task<Comment> CreateComment(Ticket ticket, User user, string message)
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
            await SendNotifications(message);
            
            return comment;
        }

        public async Task<List<Comment>> FindByTicket(Ticket ticket)
        {
            return await _databaseContext.Comments
                .Where(c => c.TicketId == ticket.Id)
                .Select(c => c)
                .ToListAsync();
        }

        private async Task SendNotifications(string message)
        {
            var regex = new Regex("(@[^ ,!.:?]+)");
            foreach (var match in regex.Matches(message))
            {
                var user = await _databaseContext.Users
                    .Where(u => u.UserName == ((Match) match).Value.Trim('@'))
                    .Select(u => u)
                    .SingleOrDefaultAsync();

                if (user == null)
                {
                    continue;
                }
                _mailingService.Send(user.Email, "You were mentioned in a comment!", message);
            }
        }
    }
}
