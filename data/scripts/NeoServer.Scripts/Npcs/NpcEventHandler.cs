using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Server.Items;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Scripts.Npcs.Modules
{
    public class NpcActionHandler
    {
        public static void OnAnswer(INpc to, ICreature from, INpcDialog dialog, string message, SpeechType type)
        {
            switch (dialog.Action)
            {
                case "shop": ShopModule.OpenShop(to, from); break;
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
