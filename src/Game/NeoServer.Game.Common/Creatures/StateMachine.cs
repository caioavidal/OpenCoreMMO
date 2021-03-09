using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Creatures
{
    public class StateMachine<T>
    {
        public T CurrentState { get; private set; }

    }
}
