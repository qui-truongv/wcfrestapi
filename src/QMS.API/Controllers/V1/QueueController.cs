using Microsoft.AspNetCore.Mvc;

namespace QMS.API.Controllers.V1
{
    public class QueueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
