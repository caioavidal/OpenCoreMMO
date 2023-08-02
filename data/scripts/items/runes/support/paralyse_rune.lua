import('NeoServer.Game.Common.Creatures', 'NeoServer.Game.Common.Creatures')
import('NeoServer.Game.Combat', 'NeoServer.Game.Combat.Conditions')

paralyseRune = {}

function paralyseRune.use(rune, usedBy, onThing)

    -- local requiredMana = 1400
    -- local paralysePeriod = 30 * 1000 --30 secs

    -- local canUseOn = {
    --     Player = 1,
    --     Monster = 2
    -- }

    -- if setContains(canUseOn, typeNameOf(onThing)) == false then -- send error message if target is not player or monster
    --     paralyseRune.sendMessageError(usedBy)
    --     return true
    -- end

    -- if usedBy.Mana < requiredMana then
    --     paralyseRune.sendMessageError(usedBy)
    --     return true
    -- end

    -- usedBy:ConsumeMana(requiredMana)

    -- local condition = Condition(CONDITION_TYPE.Paralyze, paralysePeriod)

    -- onThing:AddCondition(condition)

    return true
end


function paralyseRune.sendMessageError(usedBy)
    sendEffect(usedBy.Location, luanet.enum(EffectT, "Puff"))
    sendOperationFail(usedBy, "Sorry, not possible")
end




register({
    serverId = 2278,
    event = "use"
}, paralyseRune.use)
