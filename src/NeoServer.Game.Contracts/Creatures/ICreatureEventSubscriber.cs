using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICreatureEventSubscriber
    {
        public void Subscribe(ICreature creature);
        public void Unsubscribe(ICreature creature);
    }
}
