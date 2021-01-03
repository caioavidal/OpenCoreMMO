using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts.Items.Types.Useables
{
    public interface IUseableOnCreature: IUseableOn, IItem
    {
        /// <summary>
        /// Useable by players on creatures
        /// </summary>
        /// <param name="usedBy">creature whose item is being used</param>
        public void Use(IPlayer usedBy, ICreature creature);
    }
    public interface IUseableAttackOnCreature : IUseableOn, IItem
    {
        /// <summary>
        /// Useable by creatures to attack creatures
        /// </summary>
        /// <param name="usedBy">creature whose item is being used</param>
        public bool Use(ICreature usedBy, ICreature creature, out CombatAttackType combat);
    }
}
