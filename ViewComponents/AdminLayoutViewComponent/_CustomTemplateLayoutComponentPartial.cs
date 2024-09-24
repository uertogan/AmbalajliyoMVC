using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.ViewComponents.AdminLayoutViewComponent
{
	public class _CustomTemplateLayoutComponentPartial : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}
