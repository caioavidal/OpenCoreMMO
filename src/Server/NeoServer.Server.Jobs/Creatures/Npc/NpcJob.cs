using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Jobs.Creatures.Npc
{
    public class NpcJob
    {
        private static readonly IntervalControl interval = new(3_000);

        public static void Execute(INpc npc)
        {
            if (!interval.CanExecuteNow()) return;

            npc.Advertise();
            npc.WalkRandomStep();

            interval.MarkAsExecuted();
        }
    }
}