using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Creatures.Players;
using NeoServer.Game.Common.Texts;

namespace NeoServer.Game.Creatures.Model.Players
{
    public class PlayerParty : IPlayerParty
    {
        private readonly IPlayer _player;

        public PlayerParty(IPlayer player)
        {
            _player = player;
        }

        private uint CreatureId => _player.CreatureId;
        private IParty _partyInvite;
        public IParty Party { get; private set; }
        public bool IsInParty => Party is { };

        public void PartyEmptyHandler()
        {
            Party.OnPartyOver -= PartyEmptyHandler;
            LeaveParty();
        }

        private bool IsPartyLeader => Party?.IsLeader(_player) ?? false;

        public void InviteToParty(IPlayer invitedPlayer, IParty party)
        {
            if (invitedPlayer is null) return;
            if (invitedPlayer.CreatureId == CreatureId)
            {
                OperationFailService.Display(CreatureId, $"You cannot invite yourself.");
                return;
            }

            if (invitedPlayer.PlayerParty.IsInParty)
            {
                OperationFailService.Display(CreatureId, $"{invitedPlayer.Name} is already in a party");
                return;
            }

            var result = party.Invite(_player, invitedPlayer);

            if (!result.IsSuccess)
            {
                OperationFailService.Display(CreatureId, TextConstants.ONLY_LEADERS_CAN_INVITE_TO_PARTY);
                return;
            }

            var partyCreatedNow = Party is null;
            Party = party;
            invitedPlayer.PlayerParty.AddPartyInvite(_player, party);
            OnInviteToParty?.Invoke(_player, invitedPlayer, Party);

            if (partyCreatedNow) Party.OnPartyOver += PartyEmptyHandler;
        }

        public void AddPartyInvite(IPlayer from, IParty party)
        {
            _partyInvite = party;
            OnInvitedToParty?.Invoke(from, _player, party);
            party.OnPartyOver += RejectInvite;
        }

        public void RejectInvite()
        {
            if (_partyInvite is null) return;
            _partyInvite.OnPartyOver -= RejectInvite;

            OnRejectedPartyInvite?.Invoke(_player, _partyInvite);
            _partyInvite = null;
        }

        public void RevokePartyInvite(IPlayer invitedPlayer)
        {
            if (Party is null) return;
            Party.RevokeInvite(_player, invitedPlayer);
            OnRevokePartyInvite?.Invoke(_player, invitedPlayer, Party);
        }

        public Result LeaveParty()
        {
            if (Party is null) return Result.NotPossible;
            if (_player.InFight)
            {
                OperationFailService.Display(CreatureId, TextConstants.YOU_CANNOT_LEAVE_PARTY_WHEN_IN_FIGHT);
                return Result.Fail(InvalidOperation.CannotLeavePartyWhenInFight);
            }

            Party.OnPartyOver -= PartyEmptyHandler;

            var passedLeadership = false;
            if (IsPartyLeader) passedLeadership = Party.PassLeadership(_player).IsSuccess;

            Party?.RemoveMember(_player);

            if (passedLeadership && !Party.IsOver) OnPassedPartyLeadership?.Invoke(_player, Party.Leader, Party);

            OnLeftParty?.Invoke(_player, Party);
            Party = null;

            return Result.Success;
        }

        public Result JoinParty(IParty party)
        {
            if (party is null) return Result.NotPossible;

            var alreadyInAParty = Party is not null;
            if (alreadyInAParty)
            {
                OperationFailService.Display(CreatureId, TextConstants.ALREADY_IN_PARTY);
                return Result.Fail(InvalidOperation.AlreadyInParty);
            }

            var joinResult = party.JoinPlayer(_player);
            if (joinResult.Failed) return joinResult;

            party.OnPartyOver += PartyEmptyHandler;
            party.OnPartyOver -= RejectInvite;

            Party = party;

            OnJoinedParty?.Invoke(_player, party);
            return Result.Success;
        }

        public void PassPartyLeadership(IPlayer player)
        {
            if (Party is null) return;

            var result = player.PlayerParty.Party.ChangeLeadership(_player, player);
            if (result.IsSuccess)
            {
                OnPassedPartyLeadership?.Invoke(_player, player, Party);
                return;
            }

            switch (result.Error)
            {
                case InvalidOperation.NotAPartyMember:
                    OperationFailService.Display(CreatureId, TextConstants.PLAYER_IS_NOT_PARTY_MEMBER);
                    break;
                case InvalidOperation.NotAPartyLeader:
                    OperationFailService.Display(CreatureId, TextConstants.ONLY_LEADERS_CAN_PASS_LEADERSHIP);
                    break;
            }
        }

        public event InviteToParty OnInviteToParty;
        public event InviteToParty OnInvitedToParty;
        public event RevokePartyInvite OnRevokePartyInvite;
        public event RejectPartyInvite OnRejectedPartyInvite;
        public event JoinParty OnJoinedParty;
        public event LeaveParty OnLeftParty;
        public event PassPartyLeadership OnPassedPartyLeadership;
    }
}