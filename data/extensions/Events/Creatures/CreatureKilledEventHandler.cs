using System.Linq;
using NeoServer.Extensions.Chat;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.DataStore;

namespace NeoServer.Extensions.Events.Creatures
{
    public class CreatureKilledEventHandler : IGameEventHandler
    {
        public void Execute(ICombatActor actor, IThing by, ILoot loot)
        {
            AddDeathMessageToChannel(actor, by);
        }

        private static void AddDeathMessageToChannel(ICombatActor actor, IThing by)
        {
            if (ChatChannelStore.Data.All.FirstOrDefault(x => x is DeathChannel) is not { } deathChannel)
                return;
            if (actor is not IPlayer player) return;

            var message = $"{actor.Name} was KILLED at level {player.Level} by {by.Name}";
            deathChannel.WriteMessage(message, out var cancelMessage, SpeechType.ChannelOrangeText);
        }
    }
}