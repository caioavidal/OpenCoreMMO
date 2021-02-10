using NeoServer.Game.Contracts.Items.Types.Useables;

namespace NeoServer.Game.Contracts.Items.Types.Runes
{
    public interface IFieldRune : IUseableOnTile, IRune
    {
        string Area { get; }
        ushort Field { get; }
    }
}
