using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types.Usable;

public interface IUsable
{
    /// <summary>
    ///     A dictionary containing the use function mapped to each item type ID.
    /// </summary>
    public static readonly Dictionary<string, Action<IItem, ICreature>> UseFunctionMap = new();

    /// <summary>
    ///     Method to use the item by the player.
    /// </summary>
    /// <param name="usedBy">The player who is using the item.</param>
    void Use(IPlayer usedBy);
}