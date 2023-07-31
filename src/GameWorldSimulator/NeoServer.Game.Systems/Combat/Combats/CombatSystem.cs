using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Systems.Combat.Attacks.Item;
using AreaCombatAttack = NeoServer.Game.Systems.Combat.Attacks.AreaCombatAttack;

namespace NeoServer.Game.Systems.Combat.Combats;

public class CombatSystem
{
    private readonly IMapTool _mapTool;
    private readonly IMap _map;
    private readonly IAreaEffectStore _areaEffectStore;

    public CombatSystem(IMapTool mapTool, IMap map, IAreaEffectStore areaEffectStore )
    {
        _mapTool = mapTool;
        _map = map;
        _areaEffectStore = areaEffectStore;
    }

    public void Setup()
    {
        RuneCombatAttack.Setup(_mapTool);
        AreaRuneCombatAttack.Setup(_map, _mapTool, _areaEffectStore.Get);
        AreaCombatAttack.Setup(_map);
    }
}