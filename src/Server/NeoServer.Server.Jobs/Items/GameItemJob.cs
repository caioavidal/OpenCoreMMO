using NeoServer.Server.Contracts;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Jobs.Items
{
    public class GameItemJob
    {
        private const ushort EVENT_CHECK_ITEM_INTERVAL = 5000;
        private readonly IGameServer game;

        public GameItemJob(IGameServer game)
        {
            this.game = game;
        }

        public void StartChecking()
        {
            game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_ITEM_INTERVAL, StartChecking));

            foreach (var item in game.DecayableItemManager.Items) LiquidPoolJob.Execute(item, game);

            game.DecayableItemManager.Clean();
        }
    }
}