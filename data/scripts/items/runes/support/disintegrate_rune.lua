import('NeoServer.Game.Common.Creatures', 'NeoServer.Game.Common.Creatures')

disintegrateRune = {}

function disintegrateRune.use(rune, usedBy, onThing)

    if typeNameOf(onThing) ~= "DynamicTile" then
        sendOperationFail(usedBy, "Sorry, not possible")
        return true
    end

    local topItemOnStack = onThing.TopItemOnStack

    if topItemOnStack == nil or topItemOnStack.Metadata:HasFlag(ITEM_FLAG.Pickupable) == false then
        sendOperationFail(usedBy, "Sorry, not possible")
        return true
    end

    print('oi')

    sendEffect(onThing.Location, luanet.enum(EffectT, "Puff"))
    tile_helper.removeTopItem(onThing.Location);

    return true
end

register({
    serverId = 2310,
    event = "use"
}, disintegrateRune.use)
