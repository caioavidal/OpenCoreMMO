using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Application.Features.Combat.PlayerAttack.RuneAttack;

public static class RuneAttackCalculation
{
    public static MinMax Calculate(IPlayer player, IAttackRune rune)
    {
        var variables = rune.Variables;
        variables.TryGetValue("x", out var x);
        variables.TryGetValue("y", out var y);

        var magicLevel = player.GetSkillLevel(SkillType.Magic);

        var min = (int)(player.Level / 5 + magicLevel * Math.Min(x.Item1, x.Item2) + Math.Min(y.Item1, y.Item2));
        var max = (int)(player.Level / 5 + magicLevel * Math.Max(x.Item1, x.Item2) + Math.Min(y.Item1, y.Item2));
        return new MinMax(min, max);
    }
}