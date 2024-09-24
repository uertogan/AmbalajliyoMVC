using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.ViewComponents.AdminLayoutViewComponent
{
	public class _HeadLayoutComponentPartial : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}
