using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Model.Conditions;
using NeoServer.Game.Enums.Creatures.Players;
using NeoServer.Game.Enums.Spells;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Spells
{
    public abstract class Spell<T> : ISpell where T : Spell<T>
    {

        public abstract EffectT Effect { get; }
        public abstract uint Duration { get; }
        public virtual ushort Mana { get;  set; }
        private static readonly Lazy<T> Lazy = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
        public static T Instance => Lazy.Value;

        public abstract void OnCast(ICombatActor actor);
        public bool Invoke(ICombatActor actor, out SpellError error)
        {
            if (!CanBeUsedBy(actor, out error)) return false;
            if(actor is IPlayer player) player.ConsumeMana(Mana);
            
            OnCast(actor);
            AddCondition(actor);
            return true;
        }
        public bool CanBeUsedBy(ICombatActor actor, out SpellError error)
        {
            error = SpellError.None;

            if (actor is IPlayer player)
            {
                if (!player.HasEnoughMana(Mana))
                {
                    error = SpellError.NotEnoughMana;
                    return false;
                }
            }
            return true;
        }

        public abstract ConditionType ConditionType { get; }
        public virtual void OnEnd(ICombatActor actor) { }

        public virtual void AddCondition(ICombatActor actor)
        {
            if (ConditionType == ConditionType.None) return;

            if (actor.HasCondition(ConditionType.Haste, out var condition))
            {
                actor.AddCondition(condition);
                return;
            }

            var hasteCondition = new HasteCondition(Duration);

            hasteCondition.OnEnd = () => OnEnd(actor);

            actor.AddCondition(hasteCondition);
        }
    }

}
