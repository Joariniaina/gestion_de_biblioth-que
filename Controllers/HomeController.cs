// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
