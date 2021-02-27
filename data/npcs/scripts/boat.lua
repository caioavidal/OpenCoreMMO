register(creature);

function register(creature) 
	creature.OnHear:Add(onHear)
end

function onHear(from, receiver, speechType, message)
	print('oi');
	if string.match(message, 'teleport') then
		from:TeleportTo(from.Location.X + 1, from.Location.Y, 7)
	end
end