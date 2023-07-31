poisondagger = {}

function poisondagger.postAttack(weapon, aggressor, victim)

    local poisonCondition = {
        minDamage = 1,
        maxDamage = 1,
        damageType = DAMAGE_TYPE.Earth,
        damageCount = 10,
        interval = 4000
    }

    -- poison enemy
    causeDamageCondition(weapon, victim, poisonCondition.minDamage, poisonCondition.maxDamage, poisonCondition.damageType,
        poisonCondition.damageCount, poisonCondition.interval)

end

register({
    serverId = 2411,
    event = "postAttack"
}, poisondagger.postAttack)
