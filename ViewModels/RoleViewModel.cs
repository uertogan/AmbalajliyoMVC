using System.ComponentModel.DataAnnotations;

namespace AmbalajliyoMVC.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; } // guid string olarak temsil edildiği için

        [Required]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}
