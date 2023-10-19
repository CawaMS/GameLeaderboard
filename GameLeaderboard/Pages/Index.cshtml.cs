using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace GameLeaderboard.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly Task<RedisConnection> _redisConnectionFactory;
        private RedisConnection _redisConnection;
        public List<Player> players { get; set; }

        public IndexModel(ILogger<IndexModel> logger, Task<RedisConnection> redisConnectionFactory)
        {
            _logger = logger;
            _redisConnectionFactory = redisConnectionFactory;
        }

        public async Task OnGetAsync()
        {
            _redisConnection = await _redisConnectionFactory;

            players = await _redisConnection.BasicRetryAsync(async (db) => (await db.SortedSetRangeByRankWithScoresAsync("gameScoreSortedSet", order: Order.Descending))
            .Select(p => new Player
            {
                name = p.Element,
                score = (int) p.Score
            })
            .ToList()
            );

        }
    }
}
