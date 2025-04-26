namespace MessengerWebAPIBackend.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int UserFromId { get; set; }
        public int UserToId { get; set; }
        public string MessageText { get; set; }
        public User UserFrom { get; set; }
        public User UserTo { get; set; }
    }
}

