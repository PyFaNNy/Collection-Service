using Course_project.CloudStorage;
using Course_project.Models;
using Course_project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project.Controllers
{
    [Route("[controller]/[action]")]
    public class ItemController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ICloudStorage _cloudStorage;
        private string CREATEITEM = "Create Item";
        private string DELETEITEM = "Delete Item";
        private string EDITITEM = "Edit Item";
        public ItemController( ApplicationContext context, ICloudStorage cloudStorage)
        {
            _context = context;
            _cloudStorage = cloudStorage;
        }
        [HttpGet]
        public ActionResult Index(Guid ItemId)
        {
            Item item = _context.Items.Find(ItemId);

            var comments = _context.Comments.Where(p => p.ItemId==ItemId.ToString()).ToList();
            ViewBag.Comments = comments;
            ViewBag.Likes = _context.Likes.Where(p => p.ItemId == ItemId.ToString()).ToList().Count; ;
            return View(item);
        }
        [HttpGet]
        public IActionResult Create(string collectionId)
        {
            ViewBag.Id = collectionId;
            return View();
            }
        [HttpPost]
        public async Task<IActionResult> Create(ItemViewModel model, Guid collectionId)
        {
            if (ModelState.IsValid)
            {
                Collection collection = _context.Collections.Find(collectionId);
                collection.CountItems++;
                Item item = new Item { Name = model.Name, Description = model.Description, CollectionId = collectionId.ToString(), Img=model.Img };
                if (model.Img != null)
                {
                    await UploadFile(item);
                }
                CreateActivity(CREATEITEM, collection.Owner);
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Collections",new { collectionId });
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(Guid ItemId)
        {
            Item item = _context.Items.Find(ItemId);
            ViewBag.Item = item;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Item model, Guid itemId)
        {
            Item item = _context.Items.Find(itemId);
            item.Name = model.Name;
            Collection collection = _context.Collections.Find(item.CollectionId);
            item.Description = model.Description;
            CreateActivity(EDITITEM, collection.Owner);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Item", new { itemId });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid[] selectedItems)
        {
            string collectionId = _context.Items.Find(selectedItems[0]).CollectionId;
            Collection collection = _context.Collections.Find(new Guid(collectionId));
            foreach (var id in selectedItems)
            {
                Item item = _context.Items.Find(id);
                if (item == null)
                {
                    return NotFound();
                }
                collection.CountItems--;
                _context.Items.Remove(item);
                var likes = _context.Likes.Where(p => p.ItemId == id.ToString()).ToList();
                foreach(var like in likes)
                {
                    _context.Likes.Remove(like);
                }
                var comments = _context.Comments.Where(p => p.ItemId == id.ToString()).ToList();
                foreach (var comment in comments)
                {
                    _context.Comments.Remove(comment);
                }
                await _context.SaveChangesAsync();
            }
            CreateActivity(DELETEITEM, collection.Owner);
            return RedirectToAction("Index", "Collections", new { collectionId });
        }
        [HttpPost]
        private async Task UploadFile(Item item)
        {
            string fileNameForStorage = FormFileName(item.Name, item.Img.FileName);
            item.UrlImg = await _cloudStorage.UploadFileAsync(item.Img, fileNameForStorage);
            item.ImageStorageName = fileNameForStorage;
        }
        private static string FormFileName(string title, string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var fileNameForStorage = $"{title}-{DateTime.Now.ToString("yyyyMMddHHmmss")}{fileExtension}";
            return fileNameForStorage;
        }
        private void CreateActivity(string messenge, string userName)
        {
            var activityes = _context.RecentActivities.Where(x => x.UserName == userName).OrderByDescending(t =>t.Time).ToArray();
            if (activityes.Length < 5)
            {
                RecentActivity activity = new RecentActivity { Messenge = messenge, UserName = userName, Time = DateTime.Now };
                _context.RecentActivities.Add(activity);
            }
            else
            {
                activityes[4].Messenge = messenge;
                activityes[4].Time = DateTime.Now;
            }
            _context.SaveChanges();
        }
    }
}
