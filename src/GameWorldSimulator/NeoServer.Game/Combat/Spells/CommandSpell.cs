using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Combat.Spells;

public abstract class CommandSpell : Spell<CommandSpell>, ICommandSpell
{
    public CommandSpell()
    {
    }

    protected CommandSpell(ConditionType conditionType)
    {
        ConditionType = conditionType;
    }

    public override ConditionType ConditionType { get; }
    public override uint Duration => 0;
    public override bool ShouldSay => false;
    public object[] Params { get; set; }
    public int NumberOfParams => Params?.Length ?? 0;
    public bool HasAnyParameter => NumberOfParams > 0;
    public override EffectT Effect => EffectT.GlitterRed;
}