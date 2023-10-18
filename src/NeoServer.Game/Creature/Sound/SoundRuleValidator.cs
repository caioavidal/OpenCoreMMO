using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Creature.Sound.Rules;

namespace NeoServer.Game.Creature.Sound;

public static class SoundRuleValidator
{
    private static readonly ISoundRule[] SoundsRule = GameAssemblyCache.Cache
        .Where(p => typeof(ISoundRule).IsAssignableFrom(p) && p != typeof(ISoundRule))
        .Select(x => (ISoundRule)Activator.CreateInstance(x))
        .ToArray();

    public static bool ShouldHear(ICreature creature, ICreature spectator, SpeechType speechType)
    {
        if (SoundsRule is null) return false;

        if (creature is IMonster && spectator is not IPlayer) return false;

        foreach (var soundRule in SoundsRule)
            if (soundRule.IsApplicable(speechType) && soundRule.IsSatisfied(creature, speechType, spectator))
                return true;

        return false;
    }
}