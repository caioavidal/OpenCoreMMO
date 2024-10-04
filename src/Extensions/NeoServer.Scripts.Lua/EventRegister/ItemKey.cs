using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Scripts.Lua.EventRegister;

public readonly ref struct ItemKey
{
    public ItemKey(IItem item)
    {
        ServerId = item.ServerId;
        ActionId = item.ActionId;
        UniqueId = item.UniqueId;
    }
    public ushort ServerId { get; init; }
    public ushort ActionId { get; init; }
    public uint UniqueId { get; init; }
}