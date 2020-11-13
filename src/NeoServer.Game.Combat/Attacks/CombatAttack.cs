using NeoServer.Game.Contracts.Combat;
using System;

namespace NeoServer.Game.Combat.Attacks
{

    public class CombatAttack<T>  where T : CombatAttack<T>
    {
        private static readonly Lazy<T> Lazy = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
        public static T Instance => Lazy.Value;
    }


}
