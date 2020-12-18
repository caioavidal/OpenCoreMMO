using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.World.Tiles;

namespace NeoServer.Game.World.Map.Tiles
{
    public abstract class BaseTile : ITile
    {
        public Location Location { get; protected set; }

        public abstract IItem TopItemOnStack { get; }
        public abstract ICreature TopCreatureOnStack { get; }
        public abstract bool TryGetStackPositionOfThing(IPlayer player, IThing thing, out byte stackPosition);

        #region Store Methods
        public abstract Result CanAddThing(IThing thing, byte? slot = null);
        public abstract bool CanRemoveItem(IThing thing);
        public abstract int PossibleAmountToAdd(IThing thing);
        public abstract Result<IOperationResult> RemoveThing(IThing thing, byte amount, byte fromPosition);
        public abstract Result<IOperationResult> StoreThing(IThing thing, byte? position);
        #endregion

    }
}
