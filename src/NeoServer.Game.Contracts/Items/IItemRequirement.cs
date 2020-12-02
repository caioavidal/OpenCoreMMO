using NeoServer.Game.Common.Players;

namespace NeoServer.Game.Contracts.Items
{
    public interface IItemRequirement
    {
        VocationType Vocation { get; init; }
        ushort MinLevel { get; init; }
    }
}
