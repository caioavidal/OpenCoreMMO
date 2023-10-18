using NeoServer.Game.Common.Chats;

namespace NeoServer.Game.Common.Contracts.Creatures;

public delegate void Hear(ICreature from, ISociableCreature receiver, SpeechType speechType, string message);

public interface ISociableCreature : IWalkableCreature
{
    event Hear OnHear;
    void Hear(ICreature from, SpeechType speechType, string message);
}