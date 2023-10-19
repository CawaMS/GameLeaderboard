using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameLeaderboard.Pages
{
    public class PlayGameModel : PageModel
    {

        private readonly Task<RedisConnection> _redisConnectionFactory;
        private RedisConnection _redisConnection;

        public PlayGameModel(Task<RedisConnection> redisConnectionFactory)
        {
            _redisConnectionFactory = redisConnectionFactory;
        }
        public void OnGet()
        {         
        }

        public async void OnPostAsync()
        {
            var InputName = Request.Form["name"];
            if (!string.IsNullOrWhiteSpace(InputName))
            {
                Random rand = new Random();
                string playerName = InputName.ToString();
                Player player = new Player { 
                    name = playerName
                };
                player.score = rand.Next(0, 1000);
                ViewData["Name"] = player.name;
                ViewData["Score"] = player.score;

                _redisConnection = await _redisConnectionFactory;
                await _redisConnection.BasicRetryAsync(async (db) => await db.SortedSetUpdateAsync("gameScoreSortedSet", player.name, player.score));
            }






        }
    }
}
