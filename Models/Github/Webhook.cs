using System;

namespace hephaestus.Models
{
    public class Webhook
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public int ProjectId {get; set;}
        public Project Project {get; set;}
    }
}
