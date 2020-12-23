using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures
{
    public class CreatureFactory : ICreatureFactory
    {

        private ILiquidPoolFactory _liquidPoolFactory;

        private readonly IMap _map;
        //factories
        private readonly IPlayerFactory _playerFactory;
        private readonly IMonsterFactory _monsterFactory;
        private readonly IItemFactory itemFactory;
        private readonly IEnumerable<ICreatureEventSubscriber> creatureEventSubscribers;
        public CreatureFactory(
            IPlayerFactory playerFactory, IMonsterFactory monsterFactory, IMap map,
            ILiquidPoolFactory liquidPoolFactory, IItemFactory itemFactory, IEnumerable<ICreatureEventSubscriber> creatureEventSubscribers)
        {
            _playerFactory = playerFactory;
            _monsterFactory = monsterFactory;
            _map = map;

            _liquidPoolFactory = liquidPoolFactory;
            this.itemFactory = itemFactory;
            this.creatureEventSubscribers = creatureEventSubscribers;
        }
        public IMonster CreateMonster(string name, ISpawnPoint spawn = null)
        {
            var monster = _monsterFactory.Create(name, spawn);
            if (monster is null) return null;


            AttachEvents(monster);
            return monster;

        }
        public IPlayer CreatePlayer(IPlayerModel playerModel)
        {
            var player = _playerFactory.Create(playerModel);
            return AttachEvents(player) as IPlayer;
        }

        private ICreature AttachEvents(ICreature creature)
        {
            foreach (var subscriber in creatureEventSubscribers)
            {
                subscriber.Subscribe(creature);
            }

            if (creature is ICombatActor combatActor)
            {
                combatActor.OnKilled += AttachDeathEvent;
                combatActor.OnPropagateAttack += _map.PropagateAttack;
                combatActor.OnKilled += DetachEvents;
                combatActor.OnDamaged += AttachDamageLiquidPoolEvent;
                combatActor.OnKilled += AttachDeathLiquidPoolEvent;
            }

            return creature;
        }
        private void DetachEvents(ICombatActor creature)
        {
            if (creature is IMonster monster && !monster.IsSummon) return;

            creature.OnKilled -= DetachEvents;

            creature.OnPropagateAttack -= _map.PropagateAttack;

        }

        private void AttachDamageLiquidPoolEvent(ICombatActor enemy, ICombatActor victim, CombatDamage damage)
        {
            if (damage.IsElementalDamage) return;

            var liquidColor = victim.Blood switch
            {
                BloodType.Blood => LiquidColor.Red,
                BloodType.Slime => LiquidColor.Green
            };

            var pool = _liquidPoolFactory.CreateDamageLiquidPool(victim.Location, liquidColor);

            _map.CreateBloodPool(pool, victim.Tile);
        }
        private void AttachDeathLiquidPoolEvent(ICombatActor victim)
        {
            var liquidColor = victim.Blood switch
            {
                BloodType.Blood => LiquidColor.Red,
                BloodType.Slime => LiquidColor.Green
            };

            var pool = _liquidPoolFactory.Create(victim.Location, liquidColor);

            _map.CreateBloodPool(pool, victim.Tile);
        }

        private void AttachDeathEvent(ICreature creature)
        {
            var corpse = itemFactory.Create(creature.CorpseType, creature.Location, null);
            creature.Corpse = corpse;
            _map.ReplaceThing(creature, creature.Corpse);
        }
    }
}
