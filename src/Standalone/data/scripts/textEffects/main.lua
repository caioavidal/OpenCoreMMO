if TextEffectJob == nil then
    TextEffectJob = {}
    TextEffectJob.textEffectEventId = 0
end

function TextEffectJob.run()

    local effects = {
        { position = Location(1031, 1026, 7), text = "ISLAND!", color = 'Orange' },
        { position = Location(1035, 1026, 7), text = "CITY!",   color = 'Red' },
    }

    for i = 1, #effects do
        local effect = effects[i]
        local spectators = Map:GetPlayersAtPositionZone(effect.position)
        for player in luanet.each(spectators) do
            local result, connection = GameServer.CreatureManager:GetPlayerConnection(player.CreatureId)

            local color =  luanet.enum(TextColor, effect.color)

            --connection.OutgoingPackets:Enqueue(CreatureSayPacket(location, talkType, 'QUESTS'))
            connection.OutgoingPackets:Enqueue(AnimatedTextPacket(effect.position, color, effect.text))

            connection:Send()
        end
    end

    local interval = 5000; -- 5 seconds
    TextEffectJob.textEffectEventId = Scheduler:AddEvent(SchedulerEvent(interval, TextEffectJob.run));
end

if TextEffectJob.textEffectEventId == 0 then
    TextEffectJob.run();
end

