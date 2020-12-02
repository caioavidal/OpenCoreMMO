using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Items.Items
{
    public readonly struct ItemRequirement: IItemRequirement
    {
        public VocationType Vocation { get; init; }
        public ushort MinLevel { get; init; }
    }
}
