namespace hephaestus.Services
{
    public class CreateWebhookResponse
    {
        public class Config
        {
            public string url { get; set; }
        }
        
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public Config config { get; set; }
    }
}