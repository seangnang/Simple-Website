using Microsoft.AspNetCore.Identity;

namespace SimpleWebsite.Models
{
    public class Users : IdentityUser
    {
        public String Fullname { get; set; }
    }
}
