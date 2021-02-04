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
        public async Task<ActionResult> Index(string userId, string name, SortState sortOrder = SortState.NameAscending)
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
            IQueryable<Collection> collections = _context.Collections.Where(p => p.UserId.Equals(user.Id));
            ViewData["NameSort"] = sortOrder == SortState.NameAscending ? SortState.NameDescendingly : SortState.NameAscending;
            ViewData["CountSort"] = sortOrder == SortState.CountAscending ? SortState.CountDescendingly : SortState.CountAscending;
            ViewData["ThemeSort"] = sortOrder == SortState.ThemeAscending ? SortState.ThemeDescendingly : SortState.ThemeAscending;
            switch (sortOrder)
            {
                case SortState.NameAscending:
                    collections = collections.OrderBy(s => s.Name);
                    break;
                case SortState.NameDescendingly:
                    collections = collections.OrderByDescending(s => s.Name);
                    break;
                case SortState.CountDescendingly:
                    collections = collections.OrderByDescending(s => s.CountItems);
                    break;
                case SortState.ThemeAscending:
                    collections = collections.OrderBy(s => s.Theme);
                    break;
                case SortState.ThemeDescendingly:
                    collections = collections.OrderByDescending(s => s.Theme);
                    break;
                default:
                    collections = collections.OrderBy(s => s.CountItems);
                    break;
            }
            return View(collections.AsNoTracking().ToList());
        }

    }
}
