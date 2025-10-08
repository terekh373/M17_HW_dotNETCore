using Microsoft.AspNetCore.Mvc;

namespace M12_HW.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
