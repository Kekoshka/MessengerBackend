namespace MessengerWebAPIBackend.Models.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public ICollection<UserDTO> Users { get; set; }//Первый пользователь - отправитель, остальные получатели
        public string MessageText { get; set; }
        public DateTime PublicationDate { get; set; }
    }
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
