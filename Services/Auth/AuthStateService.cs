using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

using hephaestus.Models;

namespace hephaestus.Services
{
    public class AuthStateService : ServerAuthenticationStateProvider
    {
        private DatabaseContext _databaseContext;
        
        public AuthStateService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        public void NotifyStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<User> GetUser()
        {
            var authState = await GetAuthenticationStateAsync();
            return await _databaseContext.Users
                .Include(u => u.GithubUser)
                .Where(u => u.UserName == authState.User.Identity.Name)
                .Select(u => u)
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserWithProjects()
        {
            var authState = await GetAuthenticationStateAsync();
            return await _databaseContext.Users
                .Include(u => u.GithubUser)
                .Include(u => u.OwnedProjects)
                .Include(u => u.ContributedProjects)
                .ThenInclude(contributedProjects => contributedProjects.Project)
                .Where(u => u.UserName == authState.User.Identity.Name)
                .Select(u => u)
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserWithProjectsAndRepositories()
        {
            var authState = await GetAuthenticationStateAsync();
            return await _databaseContext.Users
                .Include(u => u.GithubUser)
                .Include(u => u.OwnedProjects)
                .Include(u => u.Invites)
                .ThenInclude(invite => invite.Project)
                .ThenInclude(project => project.Repository)
                .Include(u => u.Repositories)
                .Include(u => u.ContributedProjects)
                .ThenInclude(contributedProjects => contributedProjects.Project)
                .ThenInclude(project => project.Repository)
                .Where(u => u.UserName == authState.User.Identity.Name)
                .Select(u => u)
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserWithRepositories()
        {
            var authState = await GetAuthenticationStateAsync();
            return await _databaseContext.Users
                .Include(u => u.GithubUser)
                .Include(u => u.Repositories)
                .Where(u => u.UserName == authState.User.Identity.Name)
                .Select(u => u)
                .SingleOrDefaultAsync();
        }
    }
}
