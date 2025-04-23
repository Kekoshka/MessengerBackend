using MessengerWebAPIBackend.Context;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MessengerWebAPIBackend.Common
{
    public static class AuthOptions
    {
        public static readonly string Issuer  = "MessengerBackend";
        private static readonly string Key = "1234567890qwertyuiopasdfghjkl;zxcvbnm,./";
        public static List<string> GetAudiences()
        {
            using (var context = new ApplicationContext())
            {
                return context.Users.Select(u => u.Id.ToString()).ToList();
            }
        }
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

    }
}
