using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;

namespace NeoServer.Game.Contracts.Items.Types.Useables
{
    public delegate void UseOnTile(ICreature usedBy, ITile tile, IUseableOnTile item);

    public interface IUseableOnItem : IUseableOn, IItem
    {
        /// <summary>
        /// Useable by creatures on items (ground, weapon, stairs..)
        /// </summary>
        /// <param name="usedBy">player whose item is being used</param>
        /// <param name="item">item which will receive action</param>
        public bool Use(ICreature usedBy, IItem item);
    }
    public interface IUseableOnTile : IUseableOn, IItem
    {
        event UseOnTile OnUsedOnTile;
        
        /// <summary>
        /// Useable by creatures on items (ground, weapon, stairs..)
        /// </summary>
        /// <param name="usedBy">player whose item is being used</param>
        /// <param name="item">item which will receive action</param>
        public bool Use(ICreature usedBy, ITile item);
    }
    public interface IUseableAttackOnTile : IUseableOn, IItem
    {
        /// <summary>
        /// Useable by creatures on items (ground, weapon, stairs..)
        /// </summary>
        /// <param name="usedBy">player whose item is being used</param>
        /// <param name="item">item which will receive action</param>
        public bool Use(ICreature usedBy, ITile item, out CombatAttackType combat);
    }

}
