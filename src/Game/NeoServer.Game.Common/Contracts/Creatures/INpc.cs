using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void Answer(INpc from, ICreature to, INpcDialog dialog, string message, SpeechType type);
    public delegate IItem CreateItem(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes);
    public interface INpc : IWalkableCreature, ISociableCreature
    {
        INpcType Metadata { get; }
        event Answer OnAnswer;

        void StopTalkingToCustomer(IPlayer player);
    }
    
}
