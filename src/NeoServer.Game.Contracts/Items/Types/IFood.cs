using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IFood:IItem
    {
        public ushort Duration => Metadata.Attributes.GetAttribute<ushort>(Common.ItemAttribute.Duration);
    }
}
