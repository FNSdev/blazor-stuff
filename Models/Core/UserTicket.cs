namespace hephaestus.Models
{
    public class UserTicket
    {
        public string AssigneeId { get; set; }
        public User Assignee { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } 
    }
}
