using NeoServer.BuildingBlocks.Application;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Modules.Creatures.Npc.Routines;

public class NpcRoutine
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