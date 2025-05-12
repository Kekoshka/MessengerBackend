using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerWebAPIBackend.Models
{
    [NotMapped]
    public class SecurityData
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
