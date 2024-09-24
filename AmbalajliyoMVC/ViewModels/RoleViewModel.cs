using System.ComponentModel.DataAnnotations;

namespace AmbalajliyoMVC.ViewModels
{
    /// <summary>
    /// ViewModel for managing roles in the Ambalajliyo MVC application.
    /// </summary>
    public class RoleViewModel
    {
        public string Id { get; set; } 

        [Required]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}
