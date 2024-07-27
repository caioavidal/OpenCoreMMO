machete = {}

function machete.use(machete, usedBy, onItem)

    if not machete:CanUseOn(onItem) then
        sendOperationFail(usedBy, "Sorry, not possible.")
        return false
    end

    tileLocation = onItem.Location
    newItem = ItemService:Transform(tileLocation, 2782, 2781)
    newItem.Decayable:StartDecay()

    return true
end

register({ serverId = 2420, event = "use" }, machete.use)
