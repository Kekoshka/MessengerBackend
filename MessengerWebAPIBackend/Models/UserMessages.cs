namespace MessengerWebAPIBackend.Models
{
    public class UserMessages
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MessageId { get; set; }
        public User User { get; set; }
        public Message Message { get; set; }
    }
}
