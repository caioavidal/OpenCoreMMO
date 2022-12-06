import "NeoServer.Game.Common.Item"
import "NeoServer.Game.Common"

quest = {}

function quest.register(actionId, uniqueId)
	register("use", quest.use, actionId, uniqueId)
end

function quest.use(quest, player)
  
  local actionId = quest.Metadata.ActionId

  local item = itemFactory:Create(actionId, player.Location, nil)
  
  if item == nil or not item.IsPickupable then
    return
  end
  
  local result = addToPlayerBackpack(player, item)
  
  if not result then
    sendOperationFail(player, "You have found " .. item.FullName .. ", but you have no room to take it.");
    return
  end
  
  local notificationType = luanet.enum(NotificationType, "Information")
  
  sendNotification(player, "You have found " .. item.Metadata.FullName .. ".", notificationType);
  
end

quest.register(2404, 60047)