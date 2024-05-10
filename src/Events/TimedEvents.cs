using Astra.Database;
using Astra.Database.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Timers;
using SystemTimers = System.Timers;

namespace Astra.Events
{
    public sealed class TimedEvents
    {
        private readonly SystemTimers.Timer Timer;
        private readonly IMongoDatabase Database;
        private const int HOURLY_INTERVAL = 6;

        public TimedEvents(DatabaseEngine databaseEngine)
        {
            TimeSpan timerInterval = TimeSpan.FromMinutes(60 / HOURLY_INTERVAL);

            Timer = new SystemTimers.Timer(timerInterval)
            {
                AutoReset = true
            };
            Timer.Elapsed += async (s, e) => await TimerElapsedAsync(s, e);

            Database = databaseEngine.Database;
        }

        public void Start()
        {
            Timer.Start();
        }

        private async Task TimerElapsedAsync(object sender, ElapsedEventArgs eventArgs)
        {
            var collection = Database.GetCollection<PlanetModel>("planets");
            var filter = Builders<PlanetModel>.Filter.Ne(x => x.Colony, null);
            var planets = collection.Find(filter).ToList();

            foreach (var planet in planets)
            {
                var colony = planet.Colony;

                var payout = colony.MoneyOutput / HOURLY_INTERVAL;

                var user = await UserModel.FindUserAsync(Database, colony.Owner);
                user.Money += (long)payout;
                await user.AddAsync(Database);
            }

            Console.WriteLine("Timer function executed");
        }
    }
}
