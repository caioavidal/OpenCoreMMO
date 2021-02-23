using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.DataStore;
using NeoServer.Scripts.Chat;
using NeoServer.Server.Model.Players.Contracts;
using System.Linq;

namespace NeoServer.Scripts.Events.Creatures
{
    public class CreatureKilledEventHandler : IGameEventHandler
    {
        public void Execute(ICombatActor actor, IThing by)
        {
            AddDeathMessageToChannel(actor, by);
        }

        private static void AddDeathMessageToChannel(ICombatActor actor, IThing by)
        {
            if (ChatChannelStore.Data.All.FirstOrDefault(x => x is DeathChannel) is not IChatChannel deathChannel) return;
            if (actor is not IPlayer player) return;

            var message = $"{actor.Name} was KILLED at level {player.Level} by {by.Name}";
            deathChannel.WriteMessage(message, out var cancelMessage, SpeechType.ChannelOrangeText);
        }
    }
}
