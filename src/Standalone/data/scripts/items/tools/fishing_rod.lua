import 'System'
import ('NeoServer.Game.Common','NeoServer.Game.Common.Creatures')

local waterIds = {493, 4608, 4609, 4610, 4611, 4612, 4613, 4614, 4615, 4616, 4617, 4618, 4619, 4620, 4621, 4622, 4623, 4624, 4625, 7236, 10499}

fishingrod = {}


function fishingrod.use(fishingrod, usedBy, onItem)

	local useOnItems = make_array("ushort", waterIds)

	 if not fishingrod:CanUseOn(useOnItems, onItem) then
	     sendOperationFail(usedBy, "Sorry, not possible.")
		 return false
	 end
	
	 local effect = luanet.enum(EffectT, "RingsBlue")
	 sendEffect(onItem.Location, effect)
	 
	 return true
end

register({ serverId = 2580, event = "use" }, fishingrod.use)

