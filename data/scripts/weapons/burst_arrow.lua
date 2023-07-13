burstarrow = {}

function burstarrow.attack(ammo, aggressor, victim, combatAttackParams)

    local area = createCombatArea(victim.Location, {
        {1, 1, 1},
        {1, 3, 1},
        {1, 1, 1}
    })
    
    combatAttackParams:SetArea(area)
    combatAttackParams:SetEffect(EFFECT_TYPE.AreaFlame)

end

register({ serverId = 2546, event = "attack" }, burstarrow.attack)
