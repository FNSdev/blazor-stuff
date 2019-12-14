using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hephaestus.Models;

namespace hephaestus.Services
{
    public class TicketService 
    {
        private DatabaseContext _databaseContext;
        private UserManager<User> _userManager;
        private ToastService _toastService;
        private MailingService _mailingService;
        
        public TicketService(DatabaseContext databaseContext, ToastService toastService, UserManager<User> userManager,
            MailingService mailingService)
        {
            _databaseContext = databaseContext;
            _toastService = toastService;
            _userManager = userManager;
            _mailingService = mailingService;
        }

        public async Task<bool> CreateTicket(Project project, string name, string description)
        {
            var ticket = new Ticket
            {
                ProjectId = project.Id,
                Name = name,
                Description = description,
            };
            await _databaseContext.Tickets.AddAsync(ticket);
            await _databaseContext.SaveChangesAsync();
            await AssignUser(ticket, project.Owner);
            return true;
        }

        public async Task<bool> UpdateTicket(Ticket ticket, string name, string description, Ticket.TicketStatus status)
        {
            ticket.Name = name;
            ticket.Description = description;
            ticket.Status = status;
            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public async Task DeleteTicket(Ticket ticket)
        {
            _databaseContext.Tickets.Remove(ticket);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<bool> AssignUser(Ticket ticket, User user)
        {
            var userTicket = await _databaseContext.UserTickets
                .Where(ut => ut.Ticket.Id == ticket.Id && ut.AssigneeId == user.Id)
                .Select(ut => ut)
                .SingleOrDefaultAsync();
            if (userTicket != null)
            {
                _toastService.ShowToast("User is already assigned to this ticket", ToastLevel.Error);
                return false;
            }

            userTicket = new UserTicket
            {
                TicketId = ticket.Id,
                AssigneeId = user.Id,
            };
            await _databaseContext.UserTickets.AddAsync(userTicket);
            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RevokeUser(Ticket ticket, User user)
        {
            var userTicket = await _databaseContext.UserTickets
                .Where(ut => ut.Ticket.Id == ticket.Id && ut.AssigneeId == user.Id)
                .Select(ut => ut)
                .SingleOrDefaultAsync();
            if (userTicket == null)
            {
                _toastService.ShowToast("User is not assigned to this ticket", ToastLevel.Error);
                return false;
            }

            _databaseContext.UserTickets.Remove(userTicket);
            await _databaseContext.SaveChangesAsync();
            return true;
        }
    }
}
