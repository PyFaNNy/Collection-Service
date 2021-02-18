using Course_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project
{
    public class ChatHub : Hub
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;
        static List<User> Users = new List<User>();
        public ChatHub(ApplicationContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task Messenge(string message, string UserName)
        {
            User user = await _userManager.FindByNameAsync(UserName);
            Messenge messenge = new Messenge { Sender = UserName, Time = DateTime.Now, messenge = message, UrlImg = user.UrlImg };
            _context.Messenges.Add(messenge);
            await _context.SaveChangesAsync();
            var messenges = _context.Messenges.OrderBy(x => x.Time).ToList();
            await Clients.All.SendAsync("getMessenge", messenges);
        }
        public async Task GetUser(string userName)
        {
            User user = await _userManager.FindByNameAsync(userName);
            Users.Add(user);
            await Clients.All.SendAsync("getUsers", Users);
        }
        public async Task DelUser(string userName)
        {
            User user = await _userManager.FindByNameAsync(userName);
            Users.Remove(user);
            await Clients.All.SendAsync("getUsers", Users);
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
