using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Scripts.Chat;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Scripts.Events.Creatures
{
    public class CreatureDroppedLootEventHandler : IGameEventHandler
    {
        public void Execute(ICombatActor actor, ILoot loot, IEnumerable<ICreature> owners)
        {
            if (owners is null) return;

            foreach (var owner in owners)
            {
                if (owner is not IPlayer player) continue;

                if (player.PersonalChannels is null) continue;

                foreach (var channel in player.PersonalChannels)
                {
                    if (channel is LootChannel lootChannel) lootChannel.WriteMessage($"Loot of a {actor.Name.ToLower()}: {actor?.Corpse?.ToString()}", out var cancelMessage);
                }
            }
        }
    }
}
