using Microsoft.AspNetCore.Mvc;

namespace Course_project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
