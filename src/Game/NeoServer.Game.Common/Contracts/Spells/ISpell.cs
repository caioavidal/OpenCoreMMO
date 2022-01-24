using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Common.Contracts.Spells;

public delegate void InvokeSpell(ICombatActor creature, ISpell spell);

public interface ISpell
{
    EffectT Effect { get; }
    ushort Mana { get; set; }
    ushort MinLevel { get; set; }
    string Name { get; set; }
    uint Cooldown { get; set; }
    bool ShouldSay { get; }
    byte[] Vocations { get; set; }

    /// <summary>
    ///     Indicates if should train magic level when spell is cast
    /// </summary>
    bool IncreaseSkill => true;

    bool Invoke(ICombatActor actor, string words, out InvalidOperation error);
    bool InvokeOn(ICombatActor actor, ICombatActor onCreature, string words, out InvalidOperation error);
}

public interface ICommandSpell : ISpell
{
    public object[] Params { get; set; }
    bool ISpell.IncreaseSkill => false;
}