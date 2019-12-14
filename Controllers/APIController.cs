using System;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace hephaestus.Controllers
{
    public class APIController : Controller
    {
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public string Webhook([FromBody] string content)
        {
            return content;
        }
    }
}
