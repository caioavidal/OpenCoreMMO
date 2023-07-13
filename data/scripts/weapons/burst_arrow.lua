burstarrow = {}

function burstarrow.use(ammo, aggressor, victim, combatAttackParams)

    local area = createCombatArea(victim.Location, {
        {1, 1, 1, 1},
        {1, 3, 1, 1},
        {1, 1, 1, 1}
    })
    print(combatAttackParams)
    combatAttackParams:SetArea(area)

    combatAttackParams:SetEffect(7)
end

register({ serverId = 2546, event = "attack" }, burstarrow.use)
