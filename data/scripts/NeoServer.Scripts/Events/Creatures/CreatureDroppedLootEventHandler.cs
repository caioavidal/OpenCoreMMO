using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.DataStore;
using NeoServer.Scripts.Chat;

namespace NeoServer.Scripts.Events.Creatures
{
    public class CreatureDroppedLootEventHandler : IGameEventHandler
    {
        public void Execute(ICombatActor actor, IThing by, ILoot loot)
        {
            if (loot?.Owners is null) return;

            foreach (var owner in loot.Owners)
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
