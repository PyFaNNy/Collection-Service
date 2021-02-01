using Course_project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project.Controllers
{
    [Route("[controller]/[action]")]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext _context;
        public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: ProfileController
        public async Task<ActionResult> Index(string userId, string name)
        {
            User user = new User();
            if (userId != null)
            {
                user = await _userManager.FindByIdAsync(userId);
                ViewBag.User = user;
            }
            if (name!= null)
            {
                user = await _userManager.FindByNameAsync(name);
                ViewBag.User = user;
            }
            
            var collections = _context.Collections.Where(p => p.UserId.Equals(user.Id)).ToList();
            return View(collections);
        }

    }
}
