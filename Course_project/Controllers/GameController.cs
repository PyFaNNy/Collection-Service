using Microsoft.AspNetCore.Mvc;

namespace Course_project.Controllers
{
    public class GameController : Controller
    {
        public ActionResult Games()
        {
            return View();
        }
        public IActionResult Play()
        {
            return View();
        }
    }
}
