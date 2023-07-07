import('NeoServer.Game.Common', 'NeoServer.Game.Common.Item')

quest = {}

function quest.register(uniqueId)
    register({ uniqueId = uniqueId, event = "use" }, quest.use)  
end

function quest.use(quest, player, index)

    local questData = quest_helper.getQuestData(quest)
    if not questData then
        Logger:Error("Quest data not found")
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

    if (result.Error == "TooHeavy") then
        return sendOperationFail(player,
            "You have found " .. item_helper.concatNames(items) .. ". Weighing " .. item_helper.totalWeight(items) ..
                " oz it is too heavy.");
    end

    if (result.Error == "NotEnoughRoom") then
        return sendOperationFail(player, "You have found " .. item_helper.concatNames(items) ..
            ", but you have no room for it.");
    end

    return sendOperationFail(player, result.ErrorMessage);

end

