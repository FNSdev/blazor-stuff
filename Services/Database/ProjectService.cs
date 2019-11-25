using System.Threading.Tasks;
using hephaestus.Models;

namespace hephaestus.Services
{
    class ProjectService 
    {
        private DatabaseContext _databaseContext;

        public ProjectService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task CreateProject(string name)
        {
            var project = new Project { Name =  name };
            await _databaseContext.Projects.AddAsync(project);
        }
    }
}
