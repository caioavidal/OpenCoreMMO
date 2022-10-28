using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Game.Creatures.Services;

public class PlayerUseService : IPlayerUseService
{
    private readonly IMap _map;
    private readonly IWalkToMechanism _walkToMechanism;

    public PlayerUseService(IWalkToMechanism walkToMechanism, IMap map)
    {
        _walkToMechanism = walkToMechanism;
        _map = map;
    }

    public void Use(IPlayer player, IUsable item)
    {
        if (Guard.AnyNull(player, item)) return;

        if (!player.Location.IsNextTo(item.Location))
        {
            _walkToMechanism.WalkTo(player, () => player.Use(item), item.Location);
            return;
        }

        player.Use(item);
    }

    public void Use(IPlayer player, IUsableOn item, IThing destinationThing)
    {
        if (Guard.AnyNull(player, item, destinationThing)) return;

        if (!player.Location.IsNextTo(item.Location))
        {
            WalkToItem(player, item, destinationThing);
            return;
        }

        var itemLocation = item is IMovableItem movableItem ? movableItem.Owner.Location : item.Location;

        if (!itemLocation.IsNextTo(destinationThing.Location))
        {
            WalkToTarget(player, item, destinationThing);
            return;
        }

        UseOn(player, item, destinationThing);
    }

    private void WalkToItem(IPlayer player, IUsableOn item, IThing destinationThing)
    {
        void PickItemFromGround()
        {
            if (item.Metadata.OnUse is not null &&
                item.Metadata.OnUse.TryGetAttribute<bool>("pickfromground", out var pickFromGround) &&
                pickFromGround)
            {
                if (_map[item.Location] is not IDynamicTile dynamicTile) return;

                var result = player.PickItemFromGround(item, dynamicTile);
                if (result.Failed) return;
            }

            WalkToTarget(player, item, destinationThing);
        }

        _walkToMechanism.WalkTo(player, PickItemFromGround, item.Location);
    }

    private void WalkToTarget(IPlayer player, IUsableOn item, IThing destinationThing)
    {
        if (item.Metadata.OnUse is not null &&
            item.Metadata.OnUse.TryGetAttribute<bool>("walktotarget", out var walkToTarget) &&
            walkToTarget)
        {
            _walkToMechanism.WalkTo(player, () => UseOn(player, item, destinationThing), destinationThing.Location);
            return;
        }

        UseOn(player, item, destinationThing);
    }

    private void UseOn(IPlayer player, IUsableOn item, IThing destinationThing)
    {
        switch (destinationThing)
        {
            case ICreature creature:
                player.Use(item, creature);
                break;
            case IItem destinationItem:
                player.Use(item, destinationItem);
                break;
            case ITile destinationTile:
                player.Use(item, destinationTile);
                break;
        }
    }
}