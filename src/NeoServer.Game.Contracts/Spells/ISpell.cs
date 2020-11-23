using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures.Players;

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

        bool Invoke(ICombatActor actor, out InvalidOperation error);
        bool InvokeOn(ICombatActor actor, ICombatActor onCreature, out InvalidOperation error);
    }
}
