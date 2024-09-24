using Microsoft.AspNetCore.Mvc.Rendering;

namespace AmbalajliyoMVC.ViewModels
{
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
