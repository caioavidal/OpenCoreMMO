poisonarrow = {}

function poisonarrow.postAttack(ammo, aggressor, victim)
    -- poison enemy
    print 'poison'
end

register({ serverId = 2545, event = "postAttack" }, poisonarrow.postAttack)
