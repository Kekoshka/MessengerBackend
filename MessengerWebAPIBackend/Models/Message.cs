namespace MessengerWebAPIBackend.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ChatId { get; set; }
        public DateTime PostDate { get; set; }
        public string MessageText { get; set; }
        public Chat Chat { get; set; }
        public User User { get; set; }
    }
}
