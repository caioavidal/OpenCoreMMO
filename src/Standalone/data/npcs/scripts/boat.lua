local function onDialogAction(npc, player, dialog, action, storedValues)

	if action == "travel" then
		local cityName = storedValues['city']
		local city = npc.Metadata.CustomAttributes['custom-data'][cityName];
		local destination = city.destination;
		
		if coinTransaction:RemoveCoins(player, city.cost, true) then
			player:TeleportTo(destination.x, destination.y, destination.z)
		else
			npc:Say( npc.Metadata.CustomAttributes['custom-data']['not_enough_money'], SpeechType.PrivateNpcToPlayer, player)
			npc:BackInDialog(player, 1)
		end
		
	end
end


local function keywordHandler(message, npc, player)
	local replaced = message:gsub('|PLAYERNAME|', player.Name)
	
	local cityName = getStoredValues(npc, player, 'city')
	
	if cityName ~= nil then
		local city = npc.Metadata.CustomAttributes['custom-data'][cityName]
		replaced = replaced:gsub('|COST|', city.cost)
	end
	
	return replaced
end


function init(creature) 
	creature.OnDialogAction:Add(onDialogAction)
	creature.ReplaceKeywords = keywordHandler
end