using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.ViewComponents.AdminLayoutViewComponent
{
	public class _SidebarLayoutComponentPartial : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}
