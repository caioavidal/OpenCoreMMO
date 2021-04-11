using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Items.Items.Containers
{
    public class LootContainer : Container, ILootContainer
    {
        public LootContainer(IItemType type, Location location, IPlayer owner) : base(type, location)
        {
            Owner = owner;
            CreatedAt = DateTime.Now;
        }

        private IPlayer Owner;
        private DateTime CreatedAt;

        private bool Allowed(IPlayer player)
        {
            if (Owner is null) return true;

            if (player.Id == Owner.Id) return true;

            if ((DateTime.Now - CreatedAt).TotalSeconds > 10) return true; //todo: add 10 seconds to game configuration

            if (Owner.Party?.IsMember(player) ?? false) return true;

            return false;
        }
        public bool CanBeOpenedBy(IPlayer player) => Allowed(player);
        public bool CanBeMovedBy(IPlayer player) => Allowed(player); 
    }
}
