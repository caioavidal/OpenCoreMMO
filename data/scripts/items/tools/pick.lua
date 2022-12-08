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

register("useOnItem", pick.use, 2553)