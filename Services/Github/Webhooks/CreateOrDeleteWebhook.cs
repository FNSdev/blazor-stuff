using System.Text.Json.Serialization;

namespace hephaestus.Services.Webhooks
{
    public class CreateOrDeleteWebhook
    {
        public class Repository
        {
            public int id { get; set; }
        }
        public class Sender
        {
            public string login { get; set; }
        }
        [JsonPropertyName("ref")]
        public string ref_ { get; set; }
        public string ref_type { get; set; }
        public Sender sender { get; set; }
        public Repository repository { get; set; }
    }
}
