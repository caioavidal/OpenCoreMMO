using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Items.Items
{
    public class UseableOnItem : MoveableItem, IPickupable, IUseableOn
    {
        public UseableOnItem(IItemType type, Location location) : base(type, location)
        {
        }

        public static bool IsApplicable(IItemType type) => type.Flags.Contains(Common.ItemFlag.Useable) && type.Flags.Contains(Common.ItemFlag.Pickupable);
        public virtual void UseOn(IPlayer player, IMap map, IThing thing) { }
    }
}