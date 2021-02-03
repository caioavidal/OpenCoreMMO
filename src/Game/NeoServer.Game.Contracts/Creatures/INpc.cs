using NeoServer.Game.Common.Talks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void Answer(INpc from, ICreature to, INpcDialog dialog, string message, SpeechType type);
    public interface INpc : IWalkableCreature, ISociableCreature
    {
        INpcType Metadata { get; }

        event Answer OnAnswer;
    }
}
