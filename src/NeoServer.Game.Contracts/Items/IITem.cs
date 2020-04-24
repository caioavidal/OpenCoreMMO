using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items
{
    public interface IItem : IThing
    {
        IItemType Metadata { get; }
        string IThing.Name => Metadata.Name;
        string IThing.InspectionText => $"{Metadata.Article} {Metadata.Name}";
        string IThing.CloseInspectionText => "";
        ushort ClientId => Metadata.ClientId;
        Span<byte> GetRaw() => BitConverter.GetBytes(ClientId);

        bool HasFlag(ItemFlag flag) => Metadata.Flags.Contains(flag);

        bool CanBeMoved => HasFlag(ItemFlag.Moveable);
        bool IsBlockeable => HasFlag(ItemFlag.BlockSolid);
        bool IsCumulative => HasFlag(ItemFlag.Stackable);
        bool IsAlwaysOnTop => HasFlag(ItemFlag.AlwaysOnTop) ||
             HasFlag(ItemFlag.Clip) || HasFlag(ItemFlag.Hangable);

        bool IsPickupable => HasFlag(ItemFlag.Pickupable);
        bool IsUsable => HasFlag(ItemFlag.Useable);
        bool IsAntiProjectile => HasFlag(ItemFlag.BlockProjectTile);

        bool IsBottom => HasFlag(ItemFlag.Bottom);
        FloorChangeDirection FloorDirection => Metadata.Attributes.GetFloorChangeDirection();
    }
}
