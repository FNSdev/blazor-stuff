using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hephaestus.Models;

namespace hephaestus.Services
{
    public class ProjectService 
    {
        private DatabaseContext _databaseContext;
        private ToastService _toastService;
        private GithubService _githubService;

        public ProjectService(DatabaseContext databaseContext, ToastService toastService, GithubService githubService)
        {
            _databaseContext = databaseContext;
            _toastService = toastService;
            _githubService = githubService;
        }

        public async Task<bool> CreateProject(Project project)
        {
            var existingProject = await FindByRepository(project.Repository);

            if(existingProject != null)
            {
                _toastService.ShowToast("You already own a project for this repository", ToastLevel.Error);
                return false;
            }
            
            await _databaseContext.Projects.AddAsync(project);
            await _databaseContext.SaveChangesAsync();

            var result = await _githubService.CreateWebhook(project);
            if (result.ErrorMessage == null)
            {
                return true;
            }
            
            _toastService.ShowToast(result.ErrorMessage, ToastLevel.Error);
            return false;
        }

        public async Task<Project> FindByRepository(Repository repository)
        {
            return await _databaseContext.Projects
                .Where(p => p.Repository.Id == repository.Id)
                .Select(p => p)
                .SingleOrDefaultAsync();
        }

        public async Task<Project> FindById(int id)
        {
            return await _databaseContext.Projects
                .Include(p => p.Owner)
                .Include(p => p.Repository)
                .Include(p => p.Invites)
                .ThenInclude(invite => invite.User)
                .Include(p => p.Contributors)
                .ThenInclude(contributor => contributor.Contributor)
                .Include(p => p.Tickets)
                .ThenInclude(ticket => ticket.Assignees)
                .ThenInclude(assignee => assignee.Assignee)
                .Where(p => p.Id == id)
                .Select(p => p)
                .SingleOrDefaultAsync();
        }

        public async Task DeleteContributor(UserProject userProject)
        {
            _databaseContext.Remove(userProject);
            _databaseContext.UserTickets
                .RemoveRange(await _databaseContext.UserTickets
                    .Where(ut =>
                        ut.AssigneeId == userProject.ContributorId && ut.Ticket.ProjectId == userProject.ProjectId)
                    .Select(ut => ut).ToListAsync());
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<bool> IsContributor(Project project, User user)
        {
            var userProject = await _databaseContext.UserProjects
                .Where(up => up.ProjectId == project.Id && up.ContributorId == user.Id)
                .Select(up => up)
                .SingleOrDefaultAsync();

            return userProject != null;
        }
    }
}
