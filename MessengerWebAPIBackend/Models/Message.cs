namespace MessengerWebAPIBackend.Models
{
    public class Message
    {
        public int Id { get; set; }
        public ICollection<UserMessages> UserMessages { get; set; }//Первый пользователь - отправитель, остальные получатели
        public string MessageText { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}

