using Course_project.Models;
using Course_project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationContext _context;
        public ItemController( ApplicationContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index(Guid ItemId)
        {
            Item item = _context.Items.Find(ItemId);
            return View(item);
        }
        public IActionResult Create(string userid)
        {
            ViewBag.Id = userid;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemViewModel model, string userId)
        {
            if (ModelState.IsValid)
            {
                Item item = new Item { };
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Profile", userId);
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid[] selectedItems)
        {
            foreach (var id in selectedItems)
            {
                Item item = _context.Items.Find(id);
                if (item == null)
                {
                    return NotFound();
                }
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Collections");
        }
    }
}
