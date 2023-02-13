using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Server.Jobs.Creatures.Npc;

public class NpcJob
{
    private static readonly IntervalControl Interval = new(3_000);

    public static void Execute(INpc npc)
    {
        if (!Interval.CanExecuteNow()) return;

        npc.Advertise();
        npc.WalkRandomStep();

        Interval.MarkAsExecuted();
    }
}