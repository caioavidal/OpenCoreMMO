function getStoredValues(npc, player, name) 
	local storedValues = npc:GetPlayerStoredValues(player);
	
	if storedValues ~= nil and storedValues:ContainsKey(name) then
		return storedValues[name]
	end
	return nil
end

