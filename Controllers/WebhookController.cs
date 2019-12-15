using System;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using hephaestus.Services;
using hephaestus.Services.Webhooks;

namespace hephaestus.Controllers
{
    [IgnoreAntiforgeryToken]
    public class WebhookController : Controller
    {
        private WebhookService _webhookService;

        public WebhookController(WebhookService webhookService)
        {
            _webhookService = webhookService;
        }      
        
        [HttpPost]
        public async Task<IActionResult> Push([FromBody] PushWebhook pushWebhook)
        {
            await _webhookService.ProcessPushWebhook(pushWebhook);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] CreateOrDeleteWebhook createOrDeleteWebhook)
        {
            await _webhookService.ProcessDeleteWebhook(createOrDeleteWebhook);
            return Ok();
        }
    }
}
