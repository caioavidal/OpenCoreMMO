using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class HealCombatDefense : BaseCombatDefense
    {
        public ISpell Spell { get; }
        public HealCombatDefense(int min, int max, EffectT effect) //todo: remove dataManager from here
        {
            Spell = new HealSpell(new MinMax(min, max), effect);
        }
        public override void Defende(ICombatActor actor) => Spell?.Invoke(actor, out var error);
    }
}