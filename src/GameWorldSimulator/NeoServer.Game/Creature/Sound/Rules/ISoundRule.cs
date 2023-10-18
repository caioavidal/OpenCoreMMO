using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Creature.Sound.Rules;

internal interface ISoundRule
{
    bool IsApplicable(SpeechType type);
    bool IsSatisfied(ICreature creature, SpeechType type, ICreature spectator);
}