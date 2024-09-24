namespace AmbalajliyoMVC.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? NewPassword { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AmbalajliyoRoleId { get; set; }
        public bool? EmailConfirmed { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public bool PhoneNumberConfirmed { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;
        public bool LockoutEnabled { get; set; } = false;
        public int AccessFailedCount { get; set; } = 0;
    }
}

