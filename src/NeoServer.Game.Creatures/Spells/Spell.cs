using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Model.Conditions;
using NeoServer.Game.Enums.Creatures.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Spells
{
    public abstract class Spell<T> : ISpell where T : Spell<T>
    {
     
        protected Spell()
        {

        }
        public abstract EffectT Effect { get; }
        public abstract uint Duration { get; }

        private static readonly Lazy<T> Lazy = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
        public static T Instance => Lazy.Value;

        public abstract void Invoke(ICombatActor actor);
    }

}
