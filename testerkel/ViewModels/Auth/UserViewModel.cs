using System.ComponentModel.DataAnnotations;

namespace testerkel.ViewModels.Auth
{
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NormalizedEmail { get; set; } = string.Empty;
        public bool EnabledNotifications { get; set; }
        public RoleViewModel Role { get; set; } = new RoleViewModel();
        public string Password { get; set; } = string.Empty;
    }

    public class RoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
    }
}
