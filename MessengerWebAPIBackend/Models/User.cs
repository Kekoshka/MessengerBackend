namespace MessengerWebAPIBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime LastAction { get; set; }
        public byte[] Photo { get; set; }
    }
}
