using COMMO.Server.Data.Models.Structs;
using NeoServer.Server.Model.Creatures;
using NeoServer.Server.Model.Creatures.Contracts;
using NeoServer.Server.Model.Items;
using NeoServer.Server.Model.Items.Contracts;
using NeoServer.Server.Model.World.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Players.Contracts
{
    public interface IPlayer : ICreature
    {
        ushort Level { get; }

        byte LevelPercent { get; }

        uint Experience { get; }

        byte AccessLevel { get; } // TODO: implement.

        byte SoulPoints { get; } // TODO: nobody likes soulpoints... figure out what to do with them :)

        bool CanLogout { get; }

        Location LocationInFront { get; }

      //  IAction PendingAction { get; }

        void SetOutfit(Outfit outfit);

        uint ChooseToRemoveFromKnownSet();

        bool KnowsCreatureWithId(uint creatureId);

        byte GetSkillInfo(SkillType fist);

        byte GetSkillPercent(SkillType type);

        void AddKnownCreature(uint creatureId);

      //  void SetPendingAction(IAction action);

        void ClearPendingActions();

        void CheckInventoryContainerProximity(IThing thingChanging, ThingStateChangedEventArgs eventArgs);

        sbyte OpenContainer(IContainer thingAsContainer);

        sbyte GetContainerId(IContainer thingAsContainer);

        void CloseContainerWithId(byte openContainerIds);

        void OpenContainerAt(IContainer thingAsContainer, byte index);

        IContainer GetContainer(byte container);
    }
}
