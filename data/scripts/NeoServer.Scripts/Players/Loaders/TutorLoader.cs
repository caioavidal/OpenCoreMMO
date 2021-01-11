using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Creatures;
using NeoServer.Loaders.Interfaces;
using NeoServer.Loaders.Players;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Scripts.Players.Loaders
{
    public class TutorLoader : PlayerLoader, IPlayerLoader
    {
        private CreaturePathAccess _creaturePathAccess;
        private readonly ICreatureFactory creatureFactory;
        public TutorLoader(CreaturePathAccess creaturePathAccess, IItemFactory itemFactory, ICreatureFactory creatureFactory) : base(creaturePathAccess, itemFactory, creatureFactory)
        {
            _creaturePathAccess = creaturePathAccess;
            this.creatureFactory = creatureFactory;
        }
        public override bool IsApplicable(PlayerModel player) => player.PlayerType == 2;
        public override IPlayer Load(PlayerModel player)
        {
          
            var newPlayer = new Tutor(
                (uint)player.PlayerId,
                player.Name,
                player.Gender,
                player.Online,
                ConvertToSkills(player),
                new Outfit() { Addon = (byte)player.LookAddons, Body = (byte)player.LookBody, Feet = (byte)player.LookFeet, Head = (byte)player.LookHead, Legs = (byte)player.LookLegs, LookType = (byte)player.LookType },
                ConvertToInventory(player),
                player.Speed,
                new Location((ushort)player.PosX, (ushort)player.PosY, (byte)player.PosZ),
               _creaturePathAccess
                );

            return creatureFactory.CreatePlayer(newPlayer);
        }
    }
}
