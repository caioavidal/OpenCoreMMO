using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IConsumable: IItemRequirement, IItem
    {
        public EffectT EffecT => Metadata.Attributes.GetEffect();

        void Use(ICreature creature);
    }
}
