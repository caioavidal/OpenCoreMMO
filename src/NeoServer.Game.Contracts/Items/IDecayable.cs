using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items
{
    public interface IDecayable
    {
        int DecaysTo { get; }
        int Duration { get; }
        bool Expired { get; }
        bool StartedToDecay { get; }
        long StartedToDecayTime { get; }
        bool ShouldDisappear { get; }

        bool Decay();
    }
}
