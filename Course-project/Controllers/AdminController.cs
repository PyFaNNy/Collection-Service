using Course_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        UserManager<User> _userManager;
        SignInManager<User> _signInManager;
        private readonly ApplicationContext _context;
        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Index(SortState sortOrder = SortState.NameAscending)
        {
            IQueryable<User> users = _context.Users;
            ViewData["NameSort"] = sortOrder == SortState.NameAscending ? SortState.NameDescendingly : SortState.NameAscending;
            ViewData["EmailSort"] = sortOrder == SortState.EmailAscending ? SortState.EmailDescendingly : SortState.EmailAscending;
            ViewData["StatusSort"] = sortOrder == SortState.StatusAscending ? SortState.StatusDescendingly : SortState.StatusAscending;
            switch (sortOrder)
            {
                case SortState.NameAscending:
                    users = users.OrderBy(s => s.UserName);
                    break;
                case SortState.NameDescendingly:
                    users = users.OrderByDescending(s => s.UserName);
                    break;
                case SortState.EmailDescendingly:
                    users = users.OrderByDescending(s => s.Email);
                    break;
                case SortState.StatusAscending:
                    users = users.OrderBy(s => s.Status);
                    break;
                case SortState.StatusDescendingly:
                    users = users.OrderByDescending(s => s.Status);
                    break;
                default:
                    users = users.OrderBy(s => s.Email);
                    break;
            }
            return View(users.AsNoTracking().ToList());
        }

        public async Task<IActionResult> Block(string[] selectedUsers)
        {
            foreach (var str in selectedUsers)
            {
                User user = await _userManager.FindByNameAsync(str);
                if (user == null)
                {
                    return NotFound();
                }
                user.Status = "blocked";
                await _userManager.UpdateAsync(user);
                if (user.Status.Equals("blocked") && User.Identity.Name.Equals(user.UserName))
                {
                    await _signInManager.SignOutAsync();
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnBlock(string[] selectedUsers)
        {
            foreach (var str in selectedUsers)
            {
                User user = await _userManager.FindByNameAsync(str);
                if (user == null)
                {
                    return NotFound();
                }
                user.Status = "active";
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(string[] selectedUsers)
        {
            foreach (var str in selectedUsers)
            {
                User user = await _userManager.FindByNameAsync(str);
                if (user != null)
                {
                    if (User.Identity.Name.Equals(user.UserName))
                    {
                        await _signInManager.SignOutAsync();
                    }
                    IdentityResult result = await _userManager.DeleteAsync(user);
                }
            }
            if (_userManager.FindByNameAsync(User.Identity.Name).Status.Equals("blocked"))
            {
                return Redirect("~/Account/Logout");
            }
            return RedirectToAction("Index");
        }
    }
}