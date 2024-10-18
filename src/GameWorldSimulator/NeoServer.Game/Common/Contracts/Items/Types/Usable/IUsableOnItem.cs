using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Common.Contracts.Items.Types.Usable;

public delegate void UseOnTile(ICreature usedBy, IDynamicTile tile, IUsableOnTile item);

public interface IUsableOnItem : IUsableOn
{
    public static Func<IItem, ICreature, IItem, bool> UseFunction { get; set; }

    public Result Use(ICreature usedBy, IItem onItem)
    {
        var result = UseFunction?.Invoke(this, usedBy, onItem) ?? false;
        return result ? Result.Success : Result.NotApplicable;
    }

    bool CanUseOn(IItem onItem);
    bool CanUseOn(ushort[] items, IItem onItem);
}

public interface IUsableOnTile : IUsableOn
{
    event UseOnTile OnUsedOnTile;

    /// <summary>
    ///     Usable by creatures on items (ground, weapon, stairs..)
    /// </summary>
    /// <param name="usedBy">player whose item is being used</param>
    /// <param name="tile">tile which will receive action</param>
    public Result Use(ICreature usedBy, ITile tile);
}

public interface IUsableAttackOnTile : IUsableOn
{
    /// <summary>
    ///     Usable by creatures on items (ground, weapon, stairs..)
    /// </summary>
    /// <param name="usedBy">player whose item is being used</param>
    /// <param name="tile">tile which will receive action</param>
    /// <param name="combat"></param>
    public Result Use(ICreature usedBy, ITile tile, out CombatAttackResult combat);
}