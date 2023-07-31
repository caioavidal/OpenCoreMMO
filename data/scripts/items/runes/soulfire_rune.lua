soulfireRune = {}

function soulfireRune.use(rune, usedBy, victim)

    local poisonCondition = {
        minDamage = 10,
        maxDamage = 10,
        damageType = DAMAGE_TYPE.Fire,
        damageCount = 10,
        interval = 4000
    }

    -- poison enemy
    causeDamageCondition(rune, victim, poisonCondition.minDamage, poisonCondition.maxDamage, poisonCondition.damageType,
        poisonCondition.damageCount, poisonCondition.interval)

    return false --continue to c# method
end

register({
    serverId = 2308,
    event = "use"
}, soulfireRune.use)
