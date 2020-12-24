using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Items
{
    public interface IItemEventSubscriber
    {
        public void Subscribe(IItem item);
        public void Unsubscribe(IItem item);
    }
}
