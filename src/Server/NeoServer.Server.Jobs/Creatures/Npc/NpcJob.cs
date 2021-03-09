using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Jobs.Creatures.Npc
{
    public class NpcJob
    {
        public static void Execute(INpc npc)
        {
            npc.Advertise();
        }
    }
}
