machete = {}

function machete.register()
	register("useOnItem", machete.use, 2420)
end

function machete.use(machete, usedBy, onItem)
	
	if not machete:CanUseOn(onItem) then
	    sendOperationFail(usedBy, "Sorry, not possible.")
		return false
	end

	tileLocation = onItem.Location
	newItem = itemService:Transform(tileLocation,2782,2781)
	newItem.Decayable:StartDecay()
	
	return true
end

machete.register()