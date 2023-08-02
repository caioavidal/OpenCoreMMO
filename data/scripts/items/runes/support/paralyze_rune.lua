import('NeoServer.Game.Common.Creatures', 'NeoServer.Game.Common.Creatures')
import('NeoServer.Game.Combat', 'NeoServer.Game.Combat.Conditions')

paralyzeRune = {}

function paralyzeRune.use(rune, player, onThing)

    local requiredMana = 1400
    local paralysePeriod = 20 * 1000 --20 secs

    local canUseOn = {
        Player = 1,
        Monster = 2,
        Summon = 3
    }

    if typeNameOf(onThing) == 'DynamicTile' then
        local topCreatureOnStack = onThing.TopCreatureOnStack

        onThing = topCreatureOnStack;
    end

    if setContains(canUseOn, typeNameOf(onThing)) == false then -- send error message if target is not player or monster
        paralyseRune.sendMessageError(player)
        return true
    end

    if player.Mana < requiredMana then
        paralyseRune.sendMessageError(player)
        return true
    end

    player:ConsumeMana(requiredMana)

    local speedToReduce = onThing.Speed / 1.5

    local conditionParams = {
        type = CONDITION_TYPE.Paralyze,
        duration = 20 * 1000, --20 secs
        onStart = function() onThing:DecreaseSpeed(speedToReduce) end,
        onEnd = function() onThing:IncreaseSpeed(speedToReduce) end
    }

    local condition = createCondition(conditionParams)

    onThing:AddCondition(condition)

    sendEffect(onThing.Location, luanet.enum(EffectT, "Puff"))

    rune:Reduce();

    return true
end


function paralyzeRune.sendMessageError(usedBy)
    sendEffect(usedBy.Location, luanet.enum(EffectT, "Puff"))
    sendOperationFail(usedBy, "Sorry, not possible")
end

register({
    serverId = 2278,
    event = "use"
}, paralyzeRune.use)
