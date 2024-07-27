scythe = {}

function scythe.use(scythe, usedBy, onItem)

	if not scythe:CanUseOn(onItem) then
	    sendOperationFail(usedBy, "Sorry, not possible.")
		return false
	end
	
	local tileLocation = onItem.Location
	newItem = ItemService:Transform(tileLocation,2739,2737)
	newItem.Decayable:StartDecay()
	
	ItemService:Create(tileLocation, 2694)
	return true
end

register({ serverId = 2550, event = "use" }, scythe.use)
