using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Course_project.Models;
using Course_project.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Course_project.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        UserManager<User> _userManager;
        SignInManager<User> _signInManager;
        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index() => View(_userManager.Users.ToList());

        public async Task<IActionResult> Block(string[] selectedUsers)
        {
            foreach (var str in selectedUsers)
            {
                User user = await _userManager.FindByNameAsync(str);
                if (user == null)
                {
                    return NotFound();
                }
                user.Status = "block";
                await _userManager.UpdateAsync(user);
                if (user.Status.Equals("block") && User.Identity.Name.Equals(user.UserName))
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
            if (_userManager.FindByNameAsync(User.Identity.Name).Status.Equals("block"))
            {
                return Redirect("~/Account/Logout");
            }
            return RedirectToAction("Index");
        }
    }
}