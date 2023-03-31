bear_lever = {}

function bear_lever.use(lever, usedBy)
	
  local isOpened = lever.Metadata.TypeId == 1946
  print(isOpened)
  lever_lib.switch(lever)

  if isOpened then
     tile_helper.addItem(Location(32145,32101,11), 1304) --add stone
     return
  end
  
  print('removed')
  tile_helper.removeTopItem(Location(32145,32101,11))
    
end

--register("use", bear_lever.use, 1945,  Location(32148,32105, 11))
--register("use", bear_lever.use, 1946,  Location(32148,32105, 11))