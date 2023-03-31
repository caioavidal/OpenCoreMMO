-- Declare an empty table to hold the 'pick' function
pick = {}

function pick.use(pick, usedBy, onItem)
	
  print(onItem.ActionId)
  if onItem.ActionId ~= 100 then
    return sendOperationFail(usedBy, "You cannot use this object.")
  end

	tileLocation = onItem.Location
	newItem = itemService:Transform(tileLocation, onItem.Metadata.TypeId, 383)
	
	return true
end

-- The 'register' function takes three arguments: the item id, the name of the event, and the function to be called when the event is triggered.
register(2553, "useOnItem", pick.use)