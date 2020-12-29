using NeoServer.Enums.Creatures.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Items.Types.Useables
{
    public interface IUseableOn2: IItem
    {
        public EffectT EffecT => Metadata.Attributes.GetEffect();

    }
}
