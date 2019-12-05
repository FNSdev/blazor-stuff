using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hephaestus.Models;

namespace hephaestus.Services
{
    public class RepositoryService 
    {
        private DatabaseContext _databaseContext;
        private ProjectService _projectService;
        private ToastService _toastService;


        public RepositoryService(DatabaseContext databaseContext, ProjectService projectService, ToastService toastService)
        {
            _databaseContext = databaseContext;
            _projectService = projectService;
            _toastService = toastService;
        }

        public async Task<bool> CreateRepository(User owner, int id, string name, string description, string htmlUrl)
        {
            if(_databaseContext.Repositories.SingleOrDefault(repository => repository.Id == id && repository.Owner.Id == owner.Id) != null)
            {
                _toastService.ShowToast("Repository was already added!", ToastLevel.Error);
                return false;
            }
            else
            {
                var repository = new Repository() {
                    Owner = owner,
                    Id = id,
                    Name = name,
                    Description = description,
                    HtmlUrl = htmlUrl,
                    ProjectId = null,
                };
                
                await  _databaseContext.Repositories.AddAsync(repository);
                await _databaseContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteRepository(Repository repository)
        {
            var existingProject = await _projectService.FindByRepository(repository);
            if(existingProject != null)
            {
                _toastService.ShowToast("You have a project for this repository! It can not be deleted", ToastLevel.Error);
                return false;
            }
            else
            {
                _databaseContext.Repositories.Remove(repository);
                await _databaseContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Repository> FindRepository(int id)
        {
            return await _databaseContext.Repositories
                .Where(r => r.Id == id)
                .Select(r => r)
                .SingleOrDefaultAsync();
        }
    }
}
