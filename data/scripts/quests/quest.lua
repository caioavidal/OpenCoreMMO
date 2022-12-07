import "NeoServer.Game.Common.Item"
import "NeoServer.Game.Common"

quest = {}

function quest.register(actionId, uniqueId)
	register("use", quest.use, actionId, uniqueId)
end

function quest.use(quest, player, questData)
  
  local items = questhelper.createRewards(questData)
    
  for i=0, items.Count - 1 do
    
    local item = items[i]
     print(item.IsPickupable)
    if item and item.IsPickupable then
     
      local result = addToPlayerBackpack(player, item)
  
      if not result then
        sendOperationFail(player, "You have found " .. item.FullName .. ", but you have no room to take it.");
        return
      end
      
      local notificationType = luanet.enum(NotificationType, "Information")
  
      sendNotification(player, "You have found " .. item.Metadata.FullName .. ".", notificationType);
      
    end
  end

--  if item == nil or not item.IsPickupable then
--    return
--  end
  
--  local result = addToPlayerBackpack(player, item)
  
--  if not result then
--    sendOperationFail(player, "You have found " .. item.FullName .. ", but you have no room to take it.");
--    return
--  end
  
--  local notificationType = luanet.enum(NotificationType, "Information")
  
--  sendNotification(player, "You have found " .. item.Metadata.FullName .. ".", notificationType);
  
end

