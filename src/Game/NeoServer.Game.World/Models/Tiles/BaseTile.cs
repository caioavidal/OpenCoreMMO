using System.ComponentModel;
using NeoServer.Game.Common.Contracts.Bases;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Map.Tiles
{
    public abstract class BaseTile : Store, ITile
    {
        public Location Location { get; protected set; }

        public abstract IItem TopItemOnStack { get; }
        public abstract ICreature TopCreatureOnStack { get; }
        public abstract bool TryGetStackPositionOfThing(IPlayer player, IThing thing, out byte stackPosition);

        public abstract byte GetCreatureStackPositionIndex(IPlayer observer);
        
        private uint _flags;

        protected bool HasFlag(TileFlags flag)
        {
            return ((uint)flag & _flags) != 0;
        }

        protected void SetFlag(TileFlags flag)
        {
            _flags |= (uint)flag;
        }

        protected void RemoveFlag(TileFlags flag)
        {
            _flags &= ~(uint)flag;
        }
        public bool CannotLogout => HasFlag(TileFlags.NoLogout);
        public bool ProtectionZone => HasFlag(TileFlags.ProtectionZone);
        public FloorChangeDirection FloorDirection { get; protected set; } = FloorChangeDirection.None;

        protected void SetTileFlags(IItem item)
        {
            if (item is null) return;
            
            if (FloorDirection == FloorChangeDirection.None)
            {
                FloorDirection = item.FloorDirection;
            }
        
            if (item.Metadata.HasFlag(ItemFlag.BlockSolid) && !item.CanBeMoved) {
                SetFlag(TileFlags.ImmovableBlockSolid);
            }
        
            if (item.Metadata.HasFlag(ItemFlag.BlockPathFind)) {
                SetFlag(TileFlags.BlockPath);
            }
            
            if (item.Metadata.HasFlag(ItemFlag.BlockSolid) && item is not IMagicField) {
                SetFlag(TileFlags.NoFieldBlockPath);
                
                if(!item.CanBeMoved) SetFlag(TileFlags.ImmovableNoFieldBlockPath);
            }
            
            if (item is ITeleport) {
                SetFlag(TileFlags.Teleport);
            }
            
            if (item is IMagicField) {
                SetFlag(TileFlags.MagicField);
            }
            
            // if (item->getMailbox()) { //todo
            //     setFlag(TILESTATE_MAILBOX);
            // }
        
            // if (item->getTrashHolder()) { //todo
            //     setFlag(TILESTATE_TRASHHOLDER);
            // }
        
            if (item.Metadata.HasFlag(ItemFlag.BlockSolid)) {
                SetFlag(TileFlags.BlockSolid);
            }
        
            // if (item->getBed()) { //todo
            //     setFlag(TILESTATE_BED);
            // }
        
            
            if (item is IDepot) {
                SetFlag(TileFlags.Depot);
            }
        
            if (item.Metadata.HasFlag(ItemFlag.Hangable)) { //todo: might be wrong
                SetFlag(TileFlags.SupportsHangable);
            }
        }
    }
}