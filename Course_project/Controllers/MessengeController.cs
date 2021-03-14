using Course_project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Course_project.Controllers
{
    public class MessengeController : Controller
    {
        private readonly ApplicationContext _context;
        public MessengeController(ApplicationContext context)
        {
            _context = context;
        }
        public ActionResult Chat()
        {
            var messenges = _context.Messenges.OrderBy(x=>x.Time).ToList();
            ViewBag.Messenges = messenges;
            return View();
        }
    }
}
