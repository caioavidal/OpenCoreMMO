using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Sound;

internal class CreatureWhisperRule : ISoundRule
{
    public bool IsApplicable(SpeechType type) => type is SpeechType.Whisper;
    public bool IsSatisfied(ICreature creature, SpeechType type, ICreature spectator)
    {
        if (!creature.Location.SameFloorAs(spectator.Location)) return false;

        if (creature.Location.GetSqmDistance(spectator.Location) > 1) return false;

        return true;
    }
}