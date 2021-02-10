using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Contracts.Spells
{
    public delegate void InvokeSpell(ICombatActor creature, ISpell spell);
    public interface ISpell
    {
        EffectT Effect { get; }
        ConditionType ConditionType { get; }
        ushort Mana { get; set; }
        ushort MinLevel { get; set; }
        string Name { get; set; }
        uint Cooldown { get; set; }
        bool ShouldSay { get; }
        byte[] Vocations { get; set; }
        bool Invoke(ICombatActor actor, string words, out InvalidOperation error);
        bool InvokeOn(ICombatActor actor, ICombatActor onCreature, string words, out InvalidOperation error);
        /// <summary>
        /// Indicates if should train magic level when spell is cast
        /// </summary>
        bool IncreaseSkill => true;
    }

    public interface ICommandSpell:ISpell
    {
        public object[] Params { get; set; }
        new bool IncreaseSkill => false;
    }
}
