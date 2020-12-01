using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Items.Types.Containers
{
    public interface IPickupableContainer : IContainer, IPickupable
    {
        new float Weight { get; set; }
    }
}
