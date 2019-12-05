using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hephaestus.Models;

namespace hephaestus.Services
{
    public class InviteService 
    {
        private DatabaseContext _databaseContext;
        private UserManager<User> _userManager;
        private ToastService _toastService;
        private MailingService _mailingService;
        
        public InviteService(DatabaseContext databaseContext, ToastService toastService, UserManager<User> userManager,
            MailingService mailingService)
        {
            _databaseContext = databaseContext;
            _toastService = toastService;
            _userManager = userManager;
            _mailingService = mailingService;
        }

        public async Task<bool> CreateInvite(Project project, string userName, string message)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if(user == null)
            {
                _toastService.ShowToast("User was not found", ToastLevel.Error);
                return false;
            }
            if(await _databaseContext.Invites.Where(inv => inv.UserId == user.Id && inv.ProjectId == project.Id).Select(inv => inv).SingleOrDefaultAsync() != null)
            {
                _toastService.ShowToast("Invite was already sent", ToastLevel.Error);
                return false;                
            }
            if(await _databaseContext.UserProjects.Where(up => up.ContributorId == user.Id && up.ProjectId == project.Id).Select(up => up).SingleOrDefaultAsync() != null)
            {
                _toastService.ShowToast("User is already a part of your team", ToastLevel.Error);
                return false;                
            }
            
            var invite = new Invite() {
                Project = project,
                Message = message,
                UserId = user.Id,
                Status = Invite.InviteStatus.Pending,
            };
            await _databaseContext.Invites.AddAsync(invite);
            await _databaseContext.SaveChangesAsync();
            _toastService.ShowToast("Invite was sent successfully", ToastLevel.Success);

            var msg = $"You were invited to join {invite.Project.Name}! <p>{invite.Message}</p>";
            _mailingService.Send(invite.User.Email, "You were invited to join a new project!", msg);
            return true;
        }

        public async Task DeleteInvite(Invite invite)
        {
            _databaseContext.Invites.Remove(invite);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<bool> AcceptInvite(Invite invite)
        {
            var userProject = new UserProject() {
                ContributorId = invite.UserId,
                ProjectId = invite.ProjectId,
            };
            await _databaseContext.UserProjects.AddAsync(userProject);

            await DeleteInvite(invite);
            return true;
        }

        public async Task<bool> DeclineInvite(Invite invite)
        {
            await DeleteInvite(invite);
            return true;
        }
    }
}
