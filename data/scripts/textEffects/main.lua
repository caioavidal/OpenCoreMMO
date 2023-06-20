if TextEffectJob == nil then
    TextEffectJob = {}
    TextEffectJob.textEffectEventId = 0
end

function TextEffectJob.run()

    local effects = {
        { position = Location(986, 1208, 7), text = "HUNTS", color = 'Orange' },
        { position = Location(985, 1208, 7), text = "TRAINERS",   color = 'Orange' },
        { position = Location(983, 1208, 7), text = "VIP CAVES",   color = 'Orange' },

        { position = Location(996, 1208, 7), text = "QUESTS",   color = 'Orange' },
        { position = Location(997, 1208, 7), text = "NPCS", color = 'Orange' },
        { position = Location(998, 1208, 7), text = "BOSSES",   color = 'Orange' },
        { position = Location(999, 1208, 7), text = "CITIES",   color = 'Orange' },
        { position = Location(1000, 1208, 7), text = "DEPOT",   color = 'Orange' }
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

    local interval = 2000; -- 2 seconds
    TextEffectJob.textEffectEventId = Scheduler:AddEvent(SchedulerEvent(interval, TextEffectJob.run));
end

if TextEffectJob.textEffectEventId == 0 then
    TextEffectJob.run();
end

