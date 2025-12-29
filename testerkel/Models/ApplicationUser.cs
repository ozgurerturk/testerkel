using Microsoft.AspNetCore.Identity;

namespace testerkel.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool EnableNotifications { get; set; }
    }
}
