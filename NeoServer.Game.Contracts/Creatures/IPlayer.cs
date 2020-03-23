using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Location.Structs;

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
        ushort StaminaMinutes { get; }

        Location LocationInFront { get; }

      //  IAction PendingAction { get; }

        void ChangeOutfit(IOutfit outfit);

        uint ChooseToRemoveFromKnownSet();

        bool KnowsCreatureWithId(uint creatureId);

        byte GetSkillInfo(SkillType fist);

        byte GetSkillPercent(SkillType type);

        void AddKnownCreature(uint creatureId);

      //  void SetPendingAction(IAction action);

        void ClearPendingActions();

        void CheckInventoryContainerProximity(IThing thingChanging, IThingStateChangedEventArgs eventArgs);

        sbyte OpenContainer(IContainer thingAsContainer);

        sbyte GetContainerId(IContainer thingAsContainer);

        void CloseContainerWithId(byte openContainerIds);

        void OpenContainerAt(IContainer thingAsContainer, byte index);

        IContainer GetContainer(byte container);
    }
}
