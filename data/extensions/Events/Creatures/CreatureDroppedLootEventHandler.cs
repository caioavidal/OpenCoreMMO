using NeoServer.Extensions.Chat;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Extensions.Events.Creatures;

public class CreatureDroppedLootEventHandler : IGameEventHandler
{
    public void Execute(ICombatActor actor, IThing by, ILoot loot)
    {
        if (loot?.Owners is null) return;

        foreach (var owner in loot.Owners)
        {
            if (owner is not IPlayer player) continue;

            if (player.Channels.PersonalChannels is null) continue;

            var lootText = actor?.Corpse?.ToString() ?? "nothing";

            foreach (var channel in player.Channels.PersonalChannels)
                if (channel is LootChannel lootChannel)
                    lootChannel.WriteMessage($"Loot of a {actor.Name.ToLower()}: {lootText}",
                        out _);
        }
    }
}