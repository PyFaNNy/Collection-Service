using Course_project.Models;
using Course_project.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Course_project.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext  _context;
        private readonly SignInManager<User> _signInManager;
        public CollectionsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid collectionId)
        {
            Collection collection = _context.Collections.Find(collectionId);
            ViewBag.Collection = collection;
            var items = _context.Items.Where(p => p.CollectionId.Equals(collectionId)).ToList();
            return View(items);
        }
        public async Task<ActionResult> Collections()
        {
            return View(_context.Collections.ToList());
        }
        public IActionResult Create(string userid)
        {
            ViewBag.Id = userid;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CollectionViewModel model, string userId)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(userId);
                Collection collection = new Collection { Name = model.Name, Theme = model.Theme, Summary = model.Summary, UrlImg = model.Img, Owner = user.UserName, UserId= user.Id };
                _context.Collections.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Profile", new { userId });
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid[] selectedCollections)
        {
            string userId = _context.Collections.Find(selectedCollections[0]).UserId;
            foreach (var id in selectedCollections)
            {
                Collection collection = _context.Collections.Find(id);
                if (collection == null)
                {
                    return NotFound();
                }
                
                _context.Collections.Remove(collection);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Profile", new { userId });
        }

    }
}
