namespace MessengerWebAPIBackend.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool FaceToFace { get; set; }
        public ICollection<User> Users { get; set; }

    }
}
