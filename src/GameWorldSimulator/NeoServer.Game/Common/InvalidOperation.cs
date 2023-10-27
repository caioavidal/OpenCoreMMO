﻿using System.ComponentModel;

namespace NeoServer.Game.Common;

public enum InvalidOperation
{
    None = default,
    NotEnoughRoom,
    NotPossible,
    Impossible,
    TooHeavy,
    CannotDress,
    BothHandsNeedToBeFree,
    NotEnoughMana,
    VocationCannotUseSpell,
    NotEnoughLevel,
    Exhausted,
    CannotDrink,
    IsFull,
    TooFar,
    AlreadyInParty,
    CannotInvite,
    NotAPartyLeader,
    NotAPartyMember,
    CannotLeavePartyWhenInFight,
    NotInvited,
    PlayerNotFound,
    CreatureIsNotReachable,
    CannotAttackThatFast,
    CannotThrowThere,
    CannotUse,
    NotPermittedInProtectionZone,
    CannotAttackWhileInProtectionZone,
    CannotAttackPersonInProtectionZone,
    CreatureIsDead,
    CannotUseWeapon,
    AggressorIsNotHostile,
    CannotMove,
    AttackTargetIsInvisible
}
