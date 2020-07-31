using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location;
using System;

namespace NeoServer.Game.Contracts.Items
{
    public interface IItem : IThing
    {
        IItemType Metadata { get; }
        string IThing.Name => Metadata.Name;
        string IThing.InspectionText => $"{Metadata.Article} {Metadata.Name}";
        string IThing.CloseInspectionText => "";
        ushort ClientId => Metadata.ClientId;
        ushort TransformTo => Metadata.Attributes.GetTransformationItem();

        Span<byte> GetRaw() => BitConverter.GetBytes(ClientId);


        bool CanBeMoved => Metadata.HasFlag(ItemFlag.Moveable);
        bool IsBlockeable => Metadata.HasFlag(ItemFlag.BlockSolid);
        bool IsCumulative => Metadata.HasFlag(ItemFlag.Stackable);
        bool IsAlwaysOnTop => Metadata.HasFlag(ItemFlag.AlwaysOnTop) ||
              Metadata.HasFlag(ItemFlag.Hangable);

        bool CanHang => Metadata.HasFlag(ItemFlag.Horizontal) || Metadata.HasFlag(ItemFlag.Vertical);

        bool IsPickupable => Metadata.HasFlag(ItemFlag.Pickupable);
        bool IsUsable => Metadata.HasFlag(ItemFlag.Useable);
        bool IsAntiProjectile => Metadata.HasFlag(ItemFlag.BlockProjectTile);
        bool IsContainer => Metadata.Group == ItemGroup.GroundContainer;
        FloorChangeDirection FloorDirection => Metadata.Attributes.GetFloorChangeDirection();
    }
}
