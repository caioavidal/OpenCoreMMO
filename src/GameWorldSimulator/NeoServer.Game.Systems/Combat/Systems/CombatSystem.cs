using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Systems.Combat.Attacks;
using NeoServer.Game.Systems.Combat.Attacks.Item;
using NeoServer.Game.Systems.Combat.Attacks.Spell;

namespace NeoServer.Game.Systems.Combat.Systems;

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
        AreaCombatAttackProcessor.Setup(_map);
        SpellCombatAttack.Setup(_mapTool, _areaEffectStore);
    }
}