using NeoServer.Game.Common.Contracts.Items.Types.Usable;

namespace NeoServer.Game.Common.Contracts.Items.Types.Runes;

public interface IFieldRune : IUsableOnTile, IRune, IHasEvent
{
    string Area { get; }
    ushort Field { get; }
}