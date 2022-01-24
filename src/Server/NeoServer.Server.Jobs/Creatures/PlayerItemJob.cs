using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Server.Jobs.Creatures;

public class PlayerItemJob
{
    public static void Execute(IPlayer player)
    {
        if (player.IsDead) return;

        foreach (var item in player.Inventory.DressingItems)
            if (item is IDecayable decayable)
                decayable.TryDecay();
    }
}