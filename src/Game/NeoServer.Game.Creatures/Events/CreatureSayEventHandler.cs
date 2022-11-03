using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Creatures.Sound;

namespace NeoServer.Game.Creatures.Events;

public class CreatureSayEventHandler : IGameEventHandler
{
    private readonly IMap map;

    public CreatureSayEventHandler(IMap map)
    {
        this.map = map;
    }

    public void Execute(ICreature creature, SpeechType speechType, string message, ICreature receiver = null)
    {
        if (creature is null) return;

        if (receiver is ISociableCreature sociableCreature)
        {
            sociableCreature.Hear(creature, speechType, message);
            return;
        }

        foreach (var spectator in map.GetCreaturesAtPositionZone(creature.Location))
        {
            if (!SoundRuleValidator.ShouldHear(creature, spectator, speechType)) continue;

            if (spectator is ISociableCreature listener)
                listener.Hear(creature, speechType, message);
        }
    }
}