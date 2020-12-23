using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Items.Types
{
    public delegate void Use(IPlayer usedBy, ICreature creature, IItem item);
    public interface IConsumable: IItemRequirement, IItem
    {
        public event Use OnUsed;
        public EffectT EffecT => Metadata.Attributes.GetEffect();

        public string Sentence => Metadata.Attributes.GetAttribute(ItemAttribute.Sentence);
        void Use(IPlayer usedBy, ICreature creature);
    }
}
