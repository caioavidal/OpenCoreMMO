using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Creatures.Services;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Tests.Helpers
{
    public class PartyTestDataBuilder
    {
        /// <summary>
        /// Creates a party invite service and uses it to create a party from the provided players.
        /// </summary>
        /// <param name="partyInviteService">Optional party invite service configured to your needs. If not provided, creates a default one.</param>
        /// <returns>The created party.</returns>
        public static IParty CreateParty(IPartyInviteService partyInviteService, params IPlayer[] players)
        {
            if (players == null || players.Length <= 1)
            { 
                throw new ArgumentOutOfRangeException("Must provide at least two players to this helper to create a party."); 
            }

            partyInviteService ??= new PartyInviteService(new ChatChannelFactory()
            {
                ChannelEventSubscribers = new List<IChatChannelEventSubscriber>()
            });

            var leader = players[0];
            for (var i = 1; i < players.Length; i++)
            {
                partyInviteService.Invite(leader, players[i]);
                players[i].JoinParty(leader.Party);
            }

            return leader.Party;
        }
    }
}