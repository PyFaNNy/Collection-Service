using Course_project.Models;
using Course_project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project.Controllers
{
    [Route("[controller]/[action]")]
    public class ItemController : Controller
    {
        private readonly ApplicationContext _context;
        public ItemController( ApplicationContext context)
        {
            _context = context;
        }
        public ActionResult Index(Guid ItemId)
        {
            Item item = _context.Items.Find(ItemId);
            var comments = _context.Comments.Where(p => p.ItemId==ItemId.ToString()).ToList();
            ViewBag.Comments = comments;
            ViewBag.Likes = _context.Likes.Where(p => p.ItemId == ItemId.ToString()).ToList().Count; ;
            return View(item);
        }
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
                Item item = new Item { Name = model.Name, Description= model.Description,  CollectionId = collectionId.ToString() };
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Collections",new { collectionId });
            }
            return View(model);
        }

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
            return RedirectToAction("Index", "Collections", new { collectionId });
        }
    }
}
