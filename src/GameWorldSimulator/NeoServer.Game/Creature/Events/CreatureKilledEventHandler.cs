using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Creature.Events;

public class CreatureKilledEventHandler : IGameEventHandler
{
    private readonly IItemFactory _itemFactory;
    private readonly ILiquidPoolFactory _liquidPoolFactory;
    private readonly IMap _map;

    public CreatureKilledEventHandler(IItemFactory itemFactory, IMap map, ILiquidPoolFactory liquidPoolFactory)
    {
        _itemFactory = itemFactory;
        _map = map;
        _liquidPoolFactory = liquidPoolFactory;
    }

    public void Execute(ICreature creature, IThing by, ILoot loot)
    {
        if (creature is IMonster { IsSummon: true } monster)
        {
            //if summon just remove the creature from map
            _map.RemoveCreature(monster);
            return;
        }

        ReplaceCreatureByCorpse(creature, by, loot);
        CreateBlood(creature);
    }

    private void ReplaceCreatureByCorpse(ICreature creature, IThing by, ILoot loot)
    {
        var corpse = _itemFactory.CreateLootCorpse(creature.CorpseType, creature.Location, loot);
        creature.Corpse = corpse;

        if (creature is IWalkableCreature walkable)
        {
            walkable.Tile.AddItem(corpse);
            _map.RemoveCreature(creature);
        }
    }

    private void CreateBlood(ICreature creature)
    {
        if (creature is not ICombatActor victim) return;
        var liquidColor = victim.BloodType switch
        {
            BloodType.Blood => LiquidColor.Red,
            BloodType.Slime => LiquidColor.Green,
            _ => LiquidColor.Red
        };

        var pool = _liquidPoolFactory.Create(victim.Location, liquidColor);

        _map.CreateBloodPool(pool, victim.Tile);
    }
}