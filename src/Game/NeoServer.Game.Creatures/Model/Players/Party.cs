using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Model.Players
{
    public class Party : IParty
    {
        private readonly HashSet<uint> invites = new();
        private ushort memberCount;

        private readonly Dictionary<uint, PartyMember> members = new();

        public Party(IPlayer player, IChatChannel channel)
        {
            Leader = player;
            Channel = channel;
            player.JoinChannel(channel);
        }

        private PartyMember FirstMemberJoined
        {
            get
            {
                PartyMember partyMember = new();
                var min = uint.MaxValue;
                foreach (var member in members)
                    if (member.Value.Order < min)
                    {
                        min = member.Value.Order;
                        partyMember = member.Value;
                    }

                return partyMember;
            }
        }

        public event Action OnPartyOver;

        public IPlayer Leader { get; private set; }

        public IReadOnlyCollection<IPlayer> Members
        {
            get
            {
                var membersList = new List<IPlayer>(members.Count + 1);
                foreach (var member in members.Values) membersList.Add(member.Player);
                membersList.Add(Leader);
                return membersList;
            }
        }

        public IReadOnlyCollection<uint> Invites => invites.ToList();
        public IChatChannel Channel { get; }
        public bool IsOver => !members.Any();

        public bool IsMember(IPlayer player)
        {
            return members.ContainsKey(player.CreatureId);
        }

        public bool IsMember(uint creatureId)
        {
            return members.ContainsKey(creatureId);
        }

        public bool IsInvited(IPlayer player)
        {
            return invites.Contains(player.CreatureId);
        }

        public bool IsLeader(IPlayer player)
        {
            return player == Leader;
        }

        public bool IsLeader(uint creatureId)
        {
            return creatureId == Leader.CreatureId;
        }

        public bool JoinPlayer(IPlayer player)
        {
            if (player.IsNull()) return false;

            if (!IsInvited(player)) return false;

            invites.Remove(player.CreatureId);
            members.Add(player.CreatureId, new PartyMember(player, ++memberCount));

            player.JoinChannel(Channel);
            return true;
        }

        public Result Invite(IPlayer by, IPlayer invitedPlayer)
        {
            if (invitedPlayer.IsInParty) return new Result(InvalidOperation.CannotInvite);
            if (!IsLeader(by)) return new Result(InvalidOperation.CannotInvite);

            invites.Add(invitedPlayer.CreatureId);

            return Result.Success;
        }

        public void RevokeInvite(IPlayer by, IPlayer invitedPlayer)
        {
            if (!IsLeader(by)) return;
            if (!invites.Remove(invitedPlayer.CreatureId)) return;

            if (IsOver && !invites.Any()) OnPartyOver?.Invoke();
        }

        public void RemoveMember(IPlayer player)
        {
            if (player.IsNull()) return;
            if (player.InFight) return;

            members.Remove(player.CreatureId);
            player.ExitChannel(Channel);

            if (IsOver) OnPartyOver?.Invoke();
        }

        public Result ChangeLeadership(IPlayer from, IPlayer to)
        {
            if (Guard.AnyNull(from, to)) return Result.NotPossible;

            if (!IsMember(to)) return new Result(InvalidOperation.NotAPartyMember);
            if (!IsLeader(from)) return new Result(InvalidOperation.NotAPartyLeader);

            Leader = to;
            members.Add(from.CreatureId, new PartyMember(from, ++memberCount));
            members.Remove(to.CreatureId);

            return Result.Success;
        }

        /// <summary>
        ///     Pass leadership to first member joined
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public Result PassLeadership(IPlayer from)
        {
            if (!IsLeader(from)) return new Result(InvalidOperation.NotAPartyLeader);
            return ChangeLeadership(from, FirstMemberJoined.Player);
        }
    }

    internal readonly struct PartyMember
    {
        public PartyMember(IPlayer player, ushort order)
        {
            Player = player;
            Order = order;
        }

        public IPlayer Player { get; }
        public ushort Order { get; }
    }
}