using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace hephaestus.Models 
{
    public class DatabaseContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Invite> Invites {get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<UserTicket> UserTickets { get; set; }
        public DbSet<GithubUser> GithubUsers { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<Webhook> Webhooks { get; set; }
        
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
            
            modelBuilder.Entity<UserTicket>()
                .HasKey(userTicket => new {userTicket.AssigneeId, userTicket.TicketId });

            modelBuilder.Entity<UserTicket>()
                .HasOne(userTicket => userTicket.Assignee)
                .WithMany(assignee => assignee.Tickets)
                .HasForeignKey(userTicket => userTicket.AssigneeId);

            modelBuilder.Entity<UserTicket>()
                .HasOne(userTicket => userTicket.Ticket)
                .WithMany(ticket => ticket.Assignees)
                .HasForeignKey(userTicket => userTicket.TicketId);
        } 
    }
}
