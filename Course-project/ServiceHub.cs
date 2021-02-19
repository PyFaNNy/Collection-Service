
using Course_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Course_project
{
    public class ServiceHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationContext _context;
        private string LIKE = "Like Item";
        private string COMMENT = "Comment Item";
        public ServiceHub(ApplicationContext context, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task Comment(string message, string itemId, string UserName)
        {
            AppUser user = await _userManager.FindByNameAsync(UserName);
            Comment comment = new Comment { UserName = UserName, ItemId = itemId, messenge = message, UrlImg=user.UrlImg  };
            CreateActivity(COMMENT, UserName);
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            var comments = _context.Comments.Where(p => p.ItemId.Equals(itemId)).ToList();
            await this.Clients.Group(itemId).SendAsync("getComment", comments);
        }
        public async Task GetGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }
        public async Task DelGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }
        public async Task Like(string itemId, string UserName)
        {
            var checkedExistLike =_context.Likes.Where(p => p.ItemId == itemId && p.UserName == UserName).ToList().Count;
            if (checkedExistLike == 0)
            {
                Like like = new Like { UserName = UserName, ItemId = itemId };
                CreateActivity(LIKE, UserName);
                _context.Likes.Add(like);
                await _context.SaveChangesAsync();
            }
            var likes = _context.Likes.Where(p => p.ItemId==itemId).ToList().Count;
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
        private void CreateActivity(string messenge, string userName)
        {
            var activityes = _context.RecentActivities.Where(x => x.UserName == userName).OrderByDescending(t => t.Time).ToArray();
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
