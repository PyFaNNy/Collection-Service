using Course_project.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Course_project
{
    public class TicTacHub : Hub
    {

        private static readonly ConcurrentBag<TicTacGame> games =
             new ConcurrentBag<TicTacGame>();

        public async Task RegisterGame(string name, string email)
        {
            TicTacPlayer Player2 = new TicTacPlayer();
            TicTacPlayer Player1 = new TicTacPlayer
            {
                ConnectionId = Context.ConnectionId,
                Email = email,
                WaitingForMove = false,
                game = name
            };
            var game = new TicTacGame()
            {
                Player1 = Player1,
                Player2 = Player2,
                Name = name
            };

            games.Add(game);
            await Clients.All.SendAsync("GetGames", games.ToArray());
            await Clients.Client(Context.ConnectionId).SendAsync("Redirect", game.Name);
        }
        public async Task FindGame(string Tag)
        {
            ArrayList list = new ArrayList();
            var i = games.ToArray();
            foreach (TicTacGame fVar in i)
            {
                if (fVar.Name.Equals(Tag))
                {
                    list.Add(fVar);
                }
            }
            await Clients.Client(Context.ConnectionId).SendAsync("FindGames", list);
        }
        public async Task ConnectGame(string data)
        {
            var splitData = data.Split(' ');
            string id = splitData[0];
            string name = splitData[1];
            string email = splitData[2];
            TicTacGame game = games.First(games => games.Name.Equals(name) && games.Player1.Email.Equals(id));
            game.Player2.ConnectionId = Context.ConnectionId;
            game.Player2.WaitingForMove = true;
            game.Player2.Opponent = game.Player1;
            game.Player2.Email = email;
            game.Player2.game = game.Name;
            game.Player1.Opponent = game.Player2;


            await Clients.Client(Context.ConnectionId).SendAsync("Redirect", game.Name);
        }
        public void MakeAMove(int position, string Email)
        {
            TicTacGame game = games.First(games => games.Player1.Email.Equals(Email) || games.Player2.Email.Equals(Email));

            int symbol = 0;

            if (game.Player2.Email.Equals(Email))
            {
                symbol = 1;
            }

            var player = symbol == 0 ? game.Player1 : game.Player2;
            player.ConnectionId = Context.ConnectionId;
            if (player.WaitingForMove)
            {
                return;
            }

            if (game.Play(symbol, position))
            {
                Clients.Client(game.Player1.ConnectionId).SendAsync("GameOver", $"The winner is {player.Email}");
                Clients.Client(game.Player2.ConnectionId).SendAsync("GameOver", $"The winner is {player.Email}");
                Clients.Client(game.Player1.ConnectionId).SendAsync("moveMade", game.field);
                Clients.Client(game.Player2.ConnectionId).SendAsync("moveMade", game.field);
                Thread.Sleep(5000);
                this.Clients.Client(player.ConnectionId).SendAsync("RedirectHome");
                this.Clients.Client(player.Opponent.ConnectionId).SendAsync("RedirectHome");
                games.TryTake(out game);
            }
            Clients.Client(game.Player1.ConnectionId).SendAsync("moveMade", game.field);
            Clients.Client(game.Player2.ConnectionId).SendAsync("moveMade", game.field);


            if (!game.IsOver)
            {
                player.WaitingForMove = !player.WaitingForMove;
                player.Opponent.WaitingForMove = !player.Opponent.WaitingForMove;

                Clients.Client(player.Opponent.ConnectionId).SendAsync("WaitingForOpponent", player.Opponent.Email);
                Clients.Client(player.ConnectionId).SendAsync("WaitingForOpponent", player.Opponent.Email);
            }
        }
        public override async Task OnConnectedAsync()
        {
            await this.Clients.Caller.SendAsync("getConnected");
            await base.OnConnectedAsync();
        }
        public async Task GetCardGame(string Email)
        {
            TicTacGame game = games.First(games => games.Player1.Email.Equals(Email) || games.Player2.Email.Equals(Email));

            await Clients.Caller.SendAsync("moveMade", game.field);
        }
    }
}
