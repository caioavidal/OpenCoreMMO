import('NeoServer.Game.Creatures', 'NeoServer.Game.Creatures.Player')
import("System")

chameleonRune = {}

chameleonRune.eventId = 0

function chameleonRune.use(rune, usedBy, onThing)

    Scheduler:CancelEvent(chameleonRune.eventId)

    if (typeNameOf(onThing) == "Player" or typeNameOf(onThing) == "Summon" or typeNameOf(onThing) == "Monster" or
        typeNameOf(onThing) == "Npc") then

        sendOperationFail(usedBy, "Sorry, not possible")
        return true
    end

    if (typeNameOf(onThing) == "StaticTile") then
        sendOperationFail(usedBy, "Sorry, not possible")
        return true
    end

    local item = onThing

    if (typeNameOf(onThing) == "DynamicTile") then
        item = onThing.TopItemOnStack
    end

    if (item == nil or item.Metadata:HasFlag(ITEM_FLAG.Pickupable) == false) then
        sendOperationFail(usedBy, "Sorry, not possible")
        return true
    end

    local outfit = Outfit():LookAsAnItem(item)

    usedBy:SetTemporaryOutfit(outfit);

    chameleonRune.eventId = Scheduler:AddEvent(SchedulerEvent(5000, function()
        usedBy:BackToOldOutfit()
    end));

    return true
end

register({
    serverId = 2291,
    event = "use"
}, chameleonRune.use)
