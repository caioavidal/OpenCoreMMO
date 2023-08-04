import('NeoServer.Game.Common.Creatures', 'NeoServer.Game.Common.Creatures')

destroyFieldRune = {}

function destroyFieldRune.use(rune, usedBy, onThing)

    if typeNameOf(onThing) ~= "DynamicTile" then
        sendEffect(usedBy.Location, luanet.enum(EffectT, "Puff"))
        sendOperationFail(usedBy, "Sorry, not possible")
        return true
    end

    local hasFireField = false

    while true do

        local magicField = onThing.MagicField

        if magicField == nil then

            if hasFireField == false then --send message if there is no fire field in the ground
                sendEffect(usedBy.Location, luanet.enum(EffectT, "Puff"))
                sendOperationFail(usedBy, "Sorry, not possible")
            end

            break
        end

        hasFireField = true

        sendEffect(onThing.Location, luanet.enum(EffectT, "Puff"))
        onThing:RemoveItem(magicField)

        rune:Reduce()

    end

    return true
end

register({
    serverId = 2261,
    event = "use"
}, destroyFieldRune.use)
