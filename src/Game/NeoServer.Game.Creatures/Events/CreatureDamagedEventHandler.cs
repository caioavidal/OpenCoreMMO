using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creatures.Enums;

namespace NeoServer.Game.Creatures.Events
{
    public class CreatureDamagedEventHandler : IGameEventHandler
    {
        private readonly IMap map;
        private readonly ILiquidPoolFactory liquidPoolFactory;
        public CreatureDamagedEventHandler(IMap map, ILiquidPoolFactory liquidPoolFactory)
        {
            this.map = map;
            this.liquidPoolFactory = liquidPoolFactory;
        }
        public void Execute(IThing enemy, ICreature victim, CombatDamage damage)
        { 
            CreateBlood(victim, damage);
        }

        private void CreateBlood(ICreature creature, CombatDamage damage)
        {
            if (creature is not ICombatActor victim) return;

            if (damage.IsElementalDamage) return;

            var liquidColor = victim.Blood switch
            {
                BloodType.Blood => LiquidColor.Red,
                BloodType.Slime => LiquidColor.Green
            };

            var pool = liquidPoolFactory.CreateDamageLiquidPool(victim.Location, liquidColor);

            map.CreateBloodPool(pool, victim.Tile);
        }
    }
}
