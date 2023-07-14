poisonarrow = {}

function poisonarrow.postAttack(ammo, aggressor, victim)
    -- poison enemy
    print 'poison'

    local poisonCondition = {
        minDamage = 1,
        maxDamage = 4,
        damageType = DAMAGE_TYPE.Earth,
        damageCount = 33,
        interval = 4000
    }

    causeDamageCondition(ammo, victim, 
    poisonCondition.minDamage, poisonCondition.maxDamage, 
    poisonCondition.damageType, poisonCondition.damageCount,
    poisonCondition.interval)

end

register({
    serverId = 2545,
    event = "postAttack"
}, poisonarrow.postAttack)
