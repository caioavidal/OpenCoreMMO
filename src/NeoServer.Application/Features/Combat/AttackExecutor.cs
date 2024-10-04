using NeoServer.Application.Features.Combat.Attacks.AttackSelector;
using NeoServer.Game.Combat;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat;

public class AttackExecutor(IMap map, PlayerAttackSelector playerAttackSelector)
{
    private readonly IMap _map = map;

    public Result Execute(ICombatActor aggressor, Location location, AttackParameter attackParameter)
    {
        if (_map[location] is not IDynamicTile dynamicTile)
        {
            return Result.Fail(InvalidOperation.NotEnoughRoom);
        }
        
        if (attackParameter.Range == 0)
        {
            //melee attack
        }
        
        if (attackParameter.Area.Coordinates is not null)
        {
            //area attack
        }

        if (attackParameter.Range > 0)
        {
            //distance attack
        }
       
        return Result.Success;
    }
}