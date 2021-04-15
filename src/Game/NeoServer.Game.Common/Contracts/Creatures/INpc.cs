using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate string ReplaceKeyword(string message, object replace);
    public delegate void Answer(INpc from, ICreature to, IDialog dialog, string message, SpeechType type);
    public delegate void DialogAction(INpc from, ICreature to, IDialog dialog, string action, Dictionary<string,string> lastKeywords);
    public delegate void CustomerLeft(ICreature creature);
    
    public delegate IItem CreateItem(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes);
    public interface INpc : IWalkableCreature, ISociableCreature
    {
        INpcType Metadata { get; }
        ISpawnPoint SpawnPoint { get; }

        event Answer OnAnswer;
        event DialogAction OnDialogAction;
        event CustomerLeft OnCustomerLeft;

        void Advertise();
        void BackInDialog(ISociableCreature creature, byte count);
        void ForgetCustomer(ISociableCreature sociableCreature);
        Dictionary<string, string> GetPlayerStoredValues(ISociableCreature sociableCreature);
        void StopTalkingToCustomer(IPlayer player);
    }
    
}
