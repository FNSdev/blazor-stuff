using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using hephaestus.Models;
using hephaestus.Services.Webhooks;
using Microsoft.EntityFrameworkCore;

namespace hephaestus.Services
{
    public class WebhookService 
    {
        private DatabaseContext _databaseContext;
        private GithubService _githubService;
        private ToastService _toastService;
        private TicketService _ticketService;
        private CommentService _commentService;

        public WebhookService(DatabaseContext databaseContext, GithubService githubService, ToastService toastService,
            TicketService ticketService, CommentService commentService)
        {
            _databaseContext = databaseContext;
            _githubService = githubService;
            _toastService = toastService;
            _ticketService = ticketService;
            _commentService = commentService;
        }

        public async Task CreateWebhooks(Project project)
        {
            var webhooks = new List<Webhook>();
            var actions = new[] {"Push", "Delete"};

            foreach (var action in actions)
            {
                var result = await _githubService.CreateWebhook(project, action);
                if (result.ErrorMessage != null)
                {
                    _toastService.ShowToast(result.ErrorMessage, ToastLevel.Error);
                    return;
                }

                var webhook = new Webhook
                {
                    Id = result.Response.id,
                    Name = result.Response.name,
                    Type = result.Response.type,
                    Url = result.Response.config.url,
                    ProjectId = project.Id,
                };
                webhooks.Add(webhook);                
            }
            
            foreach (var wh in webhooks)
            {
                await _databaseContext.AddAsync(wh);
            }

            await _databaseContext.SaveChangesAsync();
        }
        
        public async Task ProcessDeleteWebhook(CreateOrDeleteWebhook createOrDeleteWebhook)
        {
            if (createOrDeleteWebhook.ref_type != "branch")
            {
                return;
            }

            var repo = await _databaseContext.Repositories
                .Where(r => r.Id == createOrDeleteWebhook.repository.id)
                .Select(r => r)
                .Include(r => r.Project)
                .SingleOrDefaultAsync();

            if (repo?.Project == null)
            {
                // TODO logging
                return;
            }

            var ticket = await _databaseContext.Tickets
                .Where(t => t.ProjectId == repo.ProjectId && t.Name == createOrDeleteWebhook.ref_)
                .Select(t => t)
                .SingleOrDefaultAsync();

            if (ticket == null)
            {
                // TODO logging
                return;
            }

            await _ticketService.DeleteTicket(ticket);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task ProcessPushWebhook(PushWebhook pushWebhook)
        {
            var regex = new Regex(@"^refs\/heads\/(.+)$");
            var match = regex.Match(pushWebhook.ref_);
            if (!match.Success)
            {
                return;
            }

            var repo = await _databaseContext.Repositories
                .Where(r => r.Id == pushWebhook.repository.id)
                .Select(r => r)
                .Include(r => r.Project)
                .SingleOrDefaultAsync();

            if (repo?.Project == null)
            {
                // TODO logging
                return;
            }
            
            var ticketName = match.Groups[1].Value;
            var ticket = await _ticketService.FindTicket(repo.Project, ticketName);
            
            if (ticket == null)
            {
                ticket = await _ticketService.CreateTicket(repo.Project, ticketName, "This ticket was created automatically. Please, adjust.");

                var githubUser = await _databaseContext.GithubUsers
                    .Select(gu => gu)
                    .Where(gu => gu.Login == pushWebhook.sender.login)
                    .Include(gu => gu.User)
                    .SingleOrDefaultAsync();

                if (githubUser != null)
                {
                    await _ticketService.AssignUser(ticket, githubUser.User);
                }
            }
            
            foreach (var commit in pushWebhook.commits)
            {
                var message = $"{commit.author.name}: {commit.message}";
                await _commentService.CreateComment(ticket, _commentService.ServiceBot, message);
            }
        }
    }
}
