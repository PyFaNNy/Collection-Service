using Course_project.CloudStorage;
using Course_project.Models;
using Course_project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Course_project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext _context;
        private readonly ICloudStorage _cloudStorage;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,ICloudStorage cloudStorage, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cloudStorage = cloudStorage;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        } 
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User 
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    Status="active"
                };
                if (model.Img != null)
                {
                    await UploadFile(user);
                }
                else
                {
                    Random rnd = new Random();
                    int a = rnd.Next(0, 16);
                    user.UrlImg= "/images/Icons/256x256/"+a.ToString()+".png";
                    user.ImageStorageName = "Default";
                }
                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, "user");
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> LoginAsync(string returnUrl = null)
        {
            var externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = externalProviders
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Name);
                if (user != null)
                {
                    if (user.Status.Equals("block"))
                    {
                        ModelState.AddModelError("", "User is blocked");
                    }
                    else
                    {
                        var result =
                                await _signInManager.PasswordSignInAsync(model.Name, model.Password, model.RememberMe, false);
                        if (result.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                            {
                                return Redirect(model.ReturnUrl);
                            }
                            else
                            {
                                await _userManager.UpdateAsync(user);
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Incorrect login and (or) password");
                        }
                    }
                }
            }
            model.ExternalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallBack), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("RegisterExternal", new ExternalLoginViewModel
            {
                ReturnUrl = returnUrl,
                Name = info.Principal.FindFirstValue(ClaimTypes.Name)
            }); ;
        }
        [AllowAnonymous]
        public IActionResult RegisterExternal(ExternalLoginViewModel model)
        {
            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterExternalConfirmed(ExternalLoginViewModel model)
        {
            var info =await _signInManager.GetExternalLoginInfoAsync();
            if (info==null)
            {
                return RedirectToAction("Login");
            }

            User user = new User
            {
                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                UserName = model.Name,
                Status = "active",
            };
            var result = await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, "user");
            if (result.Succeeded)
            {
                var identityReult = await _userManager.AddLoginAsync(user, info);
                if (identityReult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> Profile(string name)
        {
            User user = await _userManager.FindByNameAsync(name);
            ViewBag.User = user;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Edit(User model, string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (model.Email != null)
                user.Email = model.Email;
            if (model.Email != null)
                user.FirstName = model.FirstName;
            if (model.Email != null)
                user.LastName = model.LastName;
            if (model.Email != null)
                user.UserName = model.UserName;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Profile", "Profile", new { name = user.UserName });
        }
        [HttpPost]
        public async Task<ActionResult> ChangePhoto(User model, string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (model.Img != null)
            {
                user.Img = model.Img;
                await UploadFile(user);
                await _userManager.UpdateAsync(user);
                var comments = _context.Comments.Where(p => p.UserName == user.UserName).ToList();
                foreach (var comment in comments)
                {
                    comment.UrlImg = user.UrlImg;
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Profile", "Profile", new { name = user.UserName });
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
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
        [Authorize(Roles = "admin")]
        [HttpPost]
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
        [Authorize(Roles = "admin")]
        [HttpPost]
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
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult AdminPanel(SortState sortOrder = SortState.NameAscending)
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
        private async Task UploadFile(User user)
        {
            string fileNameForStorage = FormFileName(user.UserName, user.Img.FileName);
            user.UrlImg = await _cloudStorage.UploadFileAsync(user.Img, fileNameForStorage);
            user.ImageStorageName = fileNameForStorage;
        }   
        private static string FormFileName(string title, string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var fileNameForStorage = $"{title}-{DateTime.Now.ToString("yyyyMMddHHmmss")}{fileExtension}";
            return fileNameForStorage;
        }
    }
}
