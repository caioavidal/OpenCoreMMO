import "NeoServer.Game.Common.Item"
import "NeoServer.Game.Common"

quest = {}

function quest.register(actionId, uniqueId)
	register("use", quest.use, actionId, uniqueId)
end

function quest.use(quest, player, questData)
  
  if not questData then
    logger:Error("Quest data not found")
    return
  end
  
  local notificationType = luanet.enum(NotificationType, "Information")
  
  if quest_helper.checkQuestCompleted(player, questData) then
    return sendNotification(player, "The " .. quest.Metadata.Name .. " is empty.", notificationType);
  end
  
  local items = quest_helper.createRewards(questData)
    
  local result = player_helper.addToBackpack(player, items)
  
  if result.Success then
    quest_helper.setQuestAsCompleted(player, questData)
    return sendNotification(player, "You have found " .. item_helper.concatNames(items) .. ".", notificationType);
  end
  
  if(result.Error == "TooHeavy") then
    return sendOperationFail(player, "You have found " .. item_helper.concatNames(items) .. ", but you have no room to take it.");
  end
  
  if(result.Error == "NotEnoughRoom") then
     return sendOperationFail(player, "You have found " .. item_helper.concatNames(items) .. ", but you have no room to take it.");
  end
  

  return sendOperationFail(player, result.ErrorMessage);
  
end

