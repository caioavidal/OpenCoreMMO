using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IDurable
    {
        ushort Duration { get; }
        void StartDecaying();
    }
}
