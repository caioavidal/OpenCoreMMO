using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;

namespace NeoServer.Game.Creatures.Events
{
    public class CreatureSayEventHandler : IGameEventHandler
    {
        private readonly IMap map;

        public CreatureSayEventHandler(IMap map)
        {
            this.map = map;
        }

        public void Execute(ICreature creature, SpeechType type, string message, ICreature receiver = null)
        {
            if (creature is null) return;

            if (receiver is ISociableCreature sociableCreature)
            {
                sociableCreature.Hear(creature, type, message);
                return;
            }

            foreach (var spectator in map.GetCreaturesAtPositionZone(creature.Location))
                if (spectator is ISociableCreature sociableCreature1)
                    sociableCreature1.Hear(creature, type, message);
        }
    }
}