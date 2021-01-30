
using Course_project.Models;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project
{
    public class ServiceHub : Hub
    {
        private readonly ApplicationContext _context;
        public ServiceHub(ApplicationContext context)
        {
            _context = context;
        }
        public async Task Comment(string message, string itemId, string UserName)
        {
            Comment comment = new Comment {UserId = UserName, ItemId = itemId, messenge = message  };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            var comments = _context.Comments.Where(p => p.ItemId.Equals(itemId)).ToList();
            await this.Clients.All.SendAsync("getComment", comments);
        }

        public async Task Like(string message)
        {
            await this.Clients.All.SendAsync("getLike", message);
        }
    }
}
