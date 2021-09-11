using System;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IItem : IThing
    {
        /// <summary>
        ///     Item metadata. Contains a lot of information about item
        /// </summary>
        IItemType Metadata { get; }

        string CustomLookText {  get; }
        protected string LookText => CustomLookText ?? $"{Metadata.Article} {Metadata.Name}";
        string Plural => Metadata.Plural;

        ushort ClientId => Metadata.ClientId;
        ushort TransformTo => Metadata.Attributes.GetTransformationItem();

        bool CanBeMoved => Metadata.HasFlag(ItemFlag.Moveable);
        bool IsBlockeable => Metadata.HasFlag(ItemFlag.BlockSolid);
        bool BlockPathFinding => Metadata.HasFlag(ItemFlag.BlockPathFind);
        bool IsCumulative => Metadata.HasFlag(ItemFlag.Stackable);

        bool IsAlwaysOnTop => Metadata.HasFlag(ItemFlag.AlwaysOnTop) ||
                              Metadata.HasFlag(ItemFlag.Hangable);

        bool CanHang => Metadata.HasFlag(ItemFlag.Horizontal) || Metadata.HasFlag(ItemFlag.Vertical);

        bool IsPickupable => Metadata.HasFlag(ItemFlag.Pickupable);
        bool IsUsable => Metadata.HasFlag(ItemFlag.Useable);
        bool IsAntiProjectile => Metadata.HasFlag(ItemFlag.BlockProjectTile);
        bool IsContainer => Metadata.Group == ItemGroup.GroundContainer;
        FloorChangeDirection FloorDirection => Metadata.Attributes.GetFloorChangeDirection();
        string IThing.Name => Metadata.Name;
        string IThing.InspectionText => $"{LookText}";

        Span<byte> GetRaw()
        {
            return BitConverter.GetBytes(ClientId);
        }
    }
}