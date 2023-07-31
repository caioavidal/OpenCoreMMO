using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types.Usable;

public interface IUsableOnCreature : IUsableOn
{
    /// <summary>
    ///     Useable by players on creatures
    /// </summary>
    /// <param name="usedBy">creature whose item is being used</param>
    /// <param name="creature"></param>
    public void Use(IPlayer usedBy, ICreature creature);
}