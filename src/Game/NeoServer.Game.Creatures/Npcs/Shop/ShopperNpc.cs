using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Game.DataStore;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Npcs
{
    public class ShopperNpc : Npc, IShopperNpc
    {
        public event ShowShop OnShowShop;
        public ShopperNpc(INpcType type, IPathAccess pathAccess, IOutfit outfit = null, uint healthPoints = 0) : base(type, pathAccess, outfit, healthPoints)
        {
        }

        public override void SendMessageTo(ISociableCreature to, SpeechType type, INpcDialog dialog)
        {
            base.SendMessageTo(to, type, dialog);
            if (dialog.Action == "shop") ShowShopItems(to);

        }

        public virtual void ShowShopItems(ISociableCreature to)
        {
            if (to is not IPlayer player) return;

            if (!Metadata.CustomAttributes.TryGetValue("shop", out var shop)) return;

            if (shop is not IShopItem[] shopItems) return;

            player.StartShopping(this);

            OnShowShop?.Invoke(this, to, shopItems);
        }

        public virtual void StopSellingToCustomer(ISociableCreature creature)
        {
            //todo: invoke event here
        }


    }
}
