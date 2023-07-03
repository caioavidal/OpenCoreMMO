machete = {}

function machete.use(machete, usedBy, onItem)
	
	if not machete:CanUseOn(onItem) then
	    sendOperationFail(usedBy, "Sorry, not possible.")
		return false
	end

	tileLocation = onItem.Location
	newItem = ItemService:Transform(tileLocation,2782,2781)
	newItem.Decayable:StartDecay()
	
	return true
end

-- The 'register' function takes three arguments: the item id, the name of the event, and the function to be called when the event is triggered.
register("id:2420", "useOnItem", machete.use)
