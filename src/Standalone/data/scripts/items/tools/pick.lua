-- Declare an empty table to hold the 'pick' function
pick = {}

function pick.use(pick, usedBy, onItem)
	
  print(onItem.ActionId)
  if onItem.ActionId ~= 100 then
    return sendOperationFail(usedBy, "You cannot use this object.")
  end

	tileLocation = onItem.Location
	newItem = ItemService:Transform(tileLocation, onItem.Metadata.TypeId, 383)
	
	return true
end

register({ serverId = 2553, event = "use" }, pick.use)
