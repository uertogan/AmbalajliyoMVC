using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.ViewComponents.AdminLayoutViewComponent
{
	public class _MainPanelLayoutComponentPartial : ViewComponent
	{
		public IViewComponentResult Invoke () { return View(); }
	}
}
