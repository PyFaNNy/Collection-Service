using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project.Controllers
{
    public class CollectionsController : Controller
    {
        public async Task<IActionResult> Index(string[] selectedUsers)
        {
            //Возвращает вьюшку коллекции
            return View();
        }
        public async Task<IActionResult> Delete(string[] selectedUsers)
        {
            //Удаление выбранных коллекций
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Create(string[] selectedUsers)
        {
            //Возвращает вьюшку создания коллекции
            return RedirectToAction("Index");
        }
    }
}
