using Microsoft.AspNetCore.Mvc.Rendering;

namespace AmbalajliyoMVC.ViewModels
{
    /// <summary>
    /// ViewModel for managing user roles in the Ambalajliyo MVC application.
    /// </summary>
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
}
