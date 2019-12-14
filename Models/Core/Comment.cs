using System;

namespace hephaestus.Models
{
    public class Comment
    {
        public int Id {get; set;}
        public string Message {get; set;}
        public string UserId {get; set;}
        public User User {get; set;}
        public int TicketId {get; set;}
        public Ticket Ticket {get; set;}
        public DateTime CreatedAt { get; set; }
    }
}
