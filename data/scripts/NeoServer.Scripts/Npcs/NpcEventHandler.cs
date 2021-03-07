using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Scripts.Npcs.Modules
{
    public class NpcActionHandler
    {
        public static void OnAnswer(INpc to, ICreature from, IDialog dialog, string message, SpeechType type)
        {
            switch (dialog.Action)
            {
                case "teleport": Teleport(from); break;
                default:
                    break;
            }
        }

        static void Teleport(ICreature creature)
        {
            if (creature is IPlayer player)
                player.TeleportTo((player.Location + new Location(1, 0, 0)).Location);
        }
    }
}
