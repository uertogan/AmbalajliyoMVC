using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.ViewComponents.AdminLayoutViewComponent
{
	public class _FooterLayoutComponentPartial : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}
