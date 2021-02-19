using Course_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Course_project
{
    public class ChatHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationContext _context;
        static List<AppUser> Users = new List<AppUser>();
        public ChatHub(ApplicationContext context, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task Messenge(string message, string UserName)
        {
            AppUser user = await _userManager.FindByNameAsync(UserName);
            Messenge messenge = new Messenge { Sender = UserName, Time = DateTime.Now, messenge = message, UrlImg = user.UrlImg };
            _context.Messenges.Add(messenge);
            await _context.SaveChangesAsync();
            var messenges = _context.Messenges.OrderBy(x => x.Time).ToList();
            await Clients.All.SendAsync("getMessenge", messenges);
        }
        public async Task GetUser(string userName)
        {
            if (!Users.Any(x => x.UserName == userName))
            {
                AppUser user = await _userManager.FindByNameAsync(userName);
                Users.Add(user);
            }
            await Clients.All.SendAsync("getUsers", Users);
        }
        public override async Task OnConnectedAsync()
        {
            await this.Clients.Caller.SendAsync("getConnected");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception) 
        {
            AppUser user = Users.FirstOrDefault(x => x.UserName == Context.User.Identity.Name);
            Users.Remove(user);
            await Clients.All.SendAsync("getUsers", Users);
            await base.OnDisconnectedAsync(exception);
        }


    }
}
