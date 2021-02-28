function register(creature) 
	creature.OnDialogAction:Add(onDialogAction)
end

function onDialogAction(npc, player, dialog, action, lastWord)

	if action == "travel" then
		
		local data = npc.Metadata.CustomAttributes['custom-data'];
		
		for i = 0, data.Length - 1 do
			if lastWord == data[i].city then
				local destination = data[i].destination;
				player:TeleportTo(destination.x, destination.y, destination.z)
			end
		end
	
	end
end

register(creature)