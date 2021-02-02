using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void Hear(ICreature from, ISociableCreature receiver, SpeechType speechType, string message);
    public interface ISociableCreature: ICreature
    {
        event Hear OnHear;
        void Hear(ICreature from, SpeechType speechType, string message);
    }
}
