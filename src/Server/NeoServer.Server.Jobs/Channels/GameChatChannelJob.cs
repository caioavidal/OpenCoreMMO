using NeoServer.Game.DataStore;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Jobs.Items
{
    public class GameChatChannelJob
    {
        private const ushort EVENT_CHECK_ITEM_INTERVAL = 10000;
        private readonly Game game;

        public GameChatChannelJob(Game game)
        {
            this.game = game;
        }

        public void StartChecking()
        {
            game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_ITEM_INTERVAL, StartChecking));

            foreach (var channel in ChatChannelStore.Data.All)
            {
                ChatUserCleanupJob.Execute(channel);
            }
        }
    }
}
