using System.Text.Json.Serialization;

namespace hephaestus.Services.Webhooks
{
    public class PushWebhook
    {
        public class Commit
        {
            public class Author
            {
                public string name { get; set; }
            }
            public string message { get; set; }
            public Author author { get; set; }
        }

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
        public Commit[] commits { get; set; }
        public Repository repository { get; set; }
        public Sender sender { get; set; }
    }
}
