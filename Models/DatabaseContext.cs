using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace hephaestus.Models 
{
    public class DatabaseContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<Project> Projects {get; set;}
        public DbSet<Repository> Repositories {get; set;}
        public DbSet<Invite> Invites {get; set;}
        public DbSet<Problem> Problems {get; set;}

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserProject>()
                .HasKey(userProject => new {userProject.ContributorId, userProject.ProjectId });

            modelBuilder.Entity<UserProject>()
                .HasOne(userProject => userProject.Contributor)
                .WithMany(contributor => contributor.ContributedProjects)
                .HasForeignKey(userProject => userProject.ContributorId);

            modelBuilder.Entity<UserProject>()
                .HasOne(userProject => userProject.Project)
                .WithMany(project => project.Contributors)
                .HasForeignKey(userProject => userProject.ProjectId);
        } 
    }
}
