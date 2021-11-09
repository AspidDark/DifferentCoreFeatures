using Microsoft.AspNetCore.Identity;

namespace IdentityMetanit.Data
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}
