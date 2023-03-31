scythe = {}

function scythe.use(scythe, usedBy, onItem)

	if not scythe:CanUseOn(onItem) then
	    sendOperationFail(usedBy, "Sorry, not possible.")
		return false
	end
	
	tileLocation = onItem.Location
	newItem = itemService:Transform(tileLocation,2739,2737)
	newItem.Decayable:StartDecay()
	
	itemService:Create(tileLocation, 2694)
	return true
end

-- The 'register' function takes three arguments: the item id, the name of the event, and the function to be called when the event is triggered.
register(2550, "useOnItem", scythe.use)

