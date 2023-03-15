using System;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;

namespace NeoServer.Game.Common.Contracts.Items;

public interface IItem : IThing, IHasDecay
{
    /// <summary>
    ///     Item metadata. Contains a lot of information about item
    /// </summary>
    IItemType Metadata { get; }

    void UpdateMetadata(IItemType newMetadata);
    
    string InspectionText => string.Empty;
    string CloseInspectionText => string.Empty;
    string Plural => Metadata.Plural;

    ushort ClientId => Metadata.ClientId;
    ushort ServerId => Metadata.TypeId;
    ushort CanTransformTo => Metadata.Attributes.GetTransformationItem();
    bool CanBeMoved => Metadata.HasFlag(ItemFlag.Moveable);
    bool IsBlockeable => Metadata.HasFlag(ItemFlag.Unpassable);
    bool IsTransformable => CanTransformTo != default;
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

    bool HasDecayBehavior
    {
        get
        {
            var hasShowDuration =
                Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.ShowDuration, out _);
            var hasDuration = Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.Duration, out _);

            return hasShowDuration || hasDuration;
        }
    }

    string FullName => Metadata.FullName;
    ushort ActionId { get; }
    uint UniqueId { get; }
    string IThing.Name => Metadata.Name; 
    void MarkAsDeleted();
    public bool IsDeleted { get; }

    Span<byte> GetRaw()
    {
        return BitConverter.GetBytes(ClientId);
    }

    void SetActionId(ushort actionId);
    void SetUniqueId(uint uniqueId);
}