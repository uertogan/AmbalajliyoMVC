﻿namespace AmbalajliyoMVC.ViewModels
{
    public class CategoryViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
    }
}
