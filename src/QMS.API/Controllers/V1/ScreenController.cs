using Microsoft.AspNetCore.Mvc;

namespace QMS.API.Controllers.V1
{
    public class ScreenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
