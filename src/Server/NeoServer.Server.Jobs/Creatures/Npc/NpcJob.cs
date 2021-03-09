using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Jobs.Creatures.Npc
{
    public class NpcJob
    {
        static IntervalControl interval = new IntervalControl(3_000);
        public static void Execute(INpc npc)
        {
            if (!interval.CanExecuteNow()) return;

            npc.Advertise();
            npc.WalkRandomStep();

            interval.MarkAsExecuted();
        }
    }
}
