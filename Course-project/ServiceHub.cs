
using Course_project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
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
            await this.Clients.Group(itemId).SendAsync("getComment", comments);
        }

        public async Task GetGroup(string itemId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, itemId);
        }
        public async Task DelGroup(string itemId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, itemId);
        }
        public async Task Like(string itemId, string UserName)
        {
            Like like = new Like { UserId = UserName, ItemId = itemId };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            var likes = _context.Comments.Where(p => p.ItemId.Equals(itemId)).ToList().Count;
            await this.Clients.Group(itemId).SendAsync("getLike", likes);
        }
        public override async Task OnConnectedAsync()
        {
            await this.Clients.Caller.SendAsync("getConnected");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await this.Clients.Caller.SendAsync("DelConnected");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
