using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class AppUser:IdentityUser
    {
        public string FirsName { get; set; }
        public string LastName { get; set; }
    }
}
