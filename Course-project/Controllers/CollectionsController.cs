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
        private readonly SignInManager<User> _signInManager;
        public CollectionsController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
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
                Collection collection = new Collection { Name = model.Name, Theme = model.Theme, Summary = model.Summary, UrlImg = model.Img, Owner = user.UserName };
                if (user.Collections == null)
                {
                    user.Collections = new List<Collection>() { collection };
                }
                else
                {
                    user.Collections.Add(collection);
                }
                //await _userManager.(user);
                return RedirectToAction("Index", "Profile", userId);
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(Collection collection)
        {
            User user = await _userManager.FindByNameAsync(collection.Owner);
            user.Collections.Remove(collection);
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index","Profile");
        }

    }
}
