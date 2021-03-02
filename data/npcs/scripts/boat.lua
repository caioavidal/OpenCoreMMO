function register(creature) 
	creature.OnDialogAction:Add(onDialogAction)
	creature.ReplaceKeywords = keywordHandler
end

function onDialogAction(npc, player, dialog, action, storedValues)

	if action == "travel" then
	
		local city = getCityData(npc, storedValues['city'])
		local destination = city.destination;
		player:TeleportTo(destination.x, destination.y, destination.z)
	end
end

function getCityData(npc, city)

	local data = npc.Metadata.CustomAttributes['custom-data'];
		
	for i = 0, data.Length - 1 do
		if city == data[i].city then
			return data[i]
		end
	end
end

function getStoredValues(npc, player, name) 
	local storedValues = npc:GetPlayerStoredValues(player);
	
	if storedValues ~= nil and storedValues:ContainsKey(name) then
		return storedValues[name]
	end
	return nil
end

function keywordHandler(message, npc, player)
	local replaced = message:gsub('|PLAYERNAME|', player.Name)
	
	local cityName = getStoredValues(npc, player, 'city')
	
	if cityName ~= nil then
		local city = getCityData(npc, cityName)
		replaced = replaced:gsub('|COST|', city.cost)
	end
	
	return replaced
end

register(creature)