using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Creatures.Model.Players
{
    public class Party : IParty
    {
        public event Action OnPartyOver;
        public IPlayer Leader { get; private set; }
        private Dictionary<uint, PartyMember> members = new Dictionary<uint, PartyMember>();
        private HashSet<uint> invites = new HashSet<uint>();
        private ushort memberCount = 0;
        public IReadOnlyCollection<uint> Members => members.Keys.Append(Leader.CreatureId).ToList();
        public IReadOnlyCollection<uint> Invites => invites.ToList();

        public bool IsEmpty => !members.Any();

        public Party(IPlayer player)
        {
            Leader = player;
        }

        public bool IsMember(IPlayer player) => members.ContainsKey(player.CreatureId);
        public bool IsMember(uint creatureId) => members.ContainsKey(creatureId);
        public bool IsInvited(IPlayer player) => invites.Contains(player.CreatureId);
        public bool IsLeader(IPlayer player) => player == Leader;
        public bool IsLeader(uint creatureId) => creatureId == Leader.CreatureId;
        private PartyMember FirstMemberJoined
        {
            get
            {
                PartyMember partyMember = new();
                var min = uint.MaxValue;
                foreach (var member in members)
                {
                    if (member.Value.Order < min)
                    {
                        min = member.Value.Order;
                        partyMember = member.Value;
                    }
                }
                return partyMember;
            }
        }
        public bool JoinPlayer(IPlayer player)
        {
            if (Guard.AnyNull(player)) return false;

            if (!IsInvited(player)) return false;

            invites.Remove(player.CreatureId);
            members.Add(player.CreatureId, new PartyMember(player, ++memberCount));
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

            if (IsEmpty && !invites.Any()) OnPartyOver?.Invoke();

        }
        public void RemoveMember(IPlayer player)
        {
            if (player.InFight) return;

            members.Remove(player.CreatureId);
            if (IsEmpty)
            {
                OnPartyOver?.Invoke();
            }
        }

        public Result ChangeLeadership(IPlayer from, IPlayer to)
        {
            if (!IsMember(to)) return new Result(InvalidOperation.NotAPartyMember);
            if (!IsLeader(from)) return new Result(InvalidOperation.NotAPartyLeader);

            Leader = to;
            members.Remove(to.CreatureId);
            members.Add(from.CreatureId, new PartyMember(from, ++memberCount));
            return Result.Success;
        }

        /// <summary>
        /// Pass leadership to first member joined
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public Result PassLeadership(IPlayer from)
        {
            if (!IsLeader(from)) return new Result(InvalidOperation.NotAPartyLeader);

            Leader = FirstMemberJoined.Player;
            members.Remove(Leader.CreatureId);

            if (IsEmpty)
            {
                OnPartyOver?.Invoke();
                return Result.NotPossible;
            }

            return Result.Success;
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
