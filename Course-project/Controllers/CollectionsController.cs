using Course_project.CloudStorage;
using Course_project.Models;
using Course_project.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace Course_project.Controllers
{
    [Route("[controller]/[action]")]
    public class CollectionsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly ICloudStorage _cloudStorage;
        public CollectionsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context, ICloudStorage cloudStorage)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _cloudStorage = cloudStorage;
        }
        [HttpGet]
        public ActionResult Index(Guid collectionId, SortState sortOrder = SortState.NameAscending)
        {
            Collection collection = _context.Collections.Find(collectionId);
            ViewBag.Collection = collection;
            IQueryable<Item> items = _context.Items.Where(p => p.CollectionId == collectionId.ToString());
            ViewData["NameSort"] = sortOrder == SortState.NameAscending ? SortState.NameDescendingly : SortState.NameAscending;
            switch (sortOrder)
            {
                case SortState.NameAscending:
                    items = items.OrderBy(s => s.Name);
                    break;
                default:
                    items = items.OrderByDescending(s => s.Name);
                    break;
            }
            return View(items);
        }
        [HttpGet]
        public ActionResult Collections(SortState sortOrder = SortState.NameAscending)
        {
            IQueryable<Collection> collections = _context.Collections;
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
        [HttpGet]
        public IActionResult Create(string username)
        {
            ViewBag.UserName = username;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CollectionViewModel model, string username)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(username);
                Collection collection = new Collection { Name = model.Name, Theme = model.Theme, Summary = model.Summary, Owner = user.UserName, UserId = user.Id, CountItems = 0, Img = model.Img };
                if (model.Img != null)
                {
                    await UploadFile(collection);
                }
                else
                {
                    collection.UrlImg = "/images/Collections/" + model.Theme+".jpg";
                    collection.ImageStorageName = model.Theme;
                }
                _context.Collections.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Profile", new { name=username });
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid[] selectedCollections)
        {
            string name = _context.Collections.Find(selectedCollections[0]).Owner;
            foreach (var id in selectedCollections)
            {
                Collection collection = _context.Collections.Find(id);
                var items = _context.Items.Where(p => p.CollectionId == id.ToString()).ToList();
                if (collection == null)
                {
                    return NotFound();
                }
                _context.Collections.Remove(collection);
                foreach (var item in items)
                {
                    _context.Items.Remove(item);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Profile", new { name });
        }

        [HttpGet]
        public IActionResult Edit(Guid collectionId)
        {
            Collection collection = _context.Collections.Find(collectionId);
            ViewBag.Collection = collection;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Collection model, Guid collectionId)
        {
            Collection collection = _context.Collections.Find(collectionId);
            collection.Name = model.Name;
            collection.Theme = model.Theme;
            collection.Summary = model.Summary;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Collections", new { collectionId });
        }
        [HttpGet]
        public async Task<ActionResult> ProfileCollections(string name, SortState sortOrder = SortState.NameAscending)
        {
            User user = await _userManager.FindByNameAsync(name);
            ViewBag.User = user;
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
        private async Task UploadFile(Collection collection)
        {
            string fileNameForStorage = FormFileName(collection.Name, collection.Img.FileName);
            collection.UrlImg = await _cloudStorage.UploadFileAsync(collection.Img, fileNameForStorage);
            collection.ImageStorageName = fileNameForStorage;
        }

        private static string FormFileName(string title, string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var fileNameForStorage = $"{title}-{DateTime.Now.ToString("yyyyMMddHHmmss")}{fileExtension}";
            return fileNameForStorage;
        }
    }
}
