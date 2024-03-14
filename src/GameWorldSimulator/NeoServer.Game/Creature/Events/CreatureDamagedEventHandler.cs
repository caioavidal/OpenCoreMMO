using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Creature.Events;

public class CreatureDamagedEventHandler : IGameEventHandler
{
    private readonly ILiquidPoolFactory _liquidPoolFactory;
    private readonly IMap _map;

    public CreatureDamagedEventHandler(IMap map, ILiquidPoolFactory liquidPoolFactory)
    {
        _map = map;
        _liquidPoolFactory = liquidPoolFactory;
    }

    public void Execute(IThing enemy, ICreature victim, CombatDamage damage)
    {
        CreateBlood(victim, damage);
    }

    private void CreateBlood(ICreature creature, CombatDamage damage)
    {
        if (creature is not ICombatActor victim) return;

        if (damage.IsElementalDamage) return;

        var liquidColor = victim.BloodType switch
        {
            BloodType.Blood => LiquidColor.Red,
            BloodType.Slime => LiquidColor.Green,
            _ => LiquidColor.Red
        };

        var pool = _liquidPoolFactory.CreateDamageLiquidPool(victim.Location, liquidColor);

        _map.CreateBloodPool(pool, victim.Tile);
    }
}