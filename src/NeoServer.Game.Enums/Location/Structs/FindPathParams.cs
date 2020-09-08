using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums.Location.Structs
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]
    public readonly ref struct FindPathParams
    {
      
        public FindPathParams(bool fullPathSearch, bool clearSight, bool allowDiagonal, bool keepDistance, int maxSearchDist, int minTargetDist, int maxTargetDist)
        {
            FullPathSearch = fullPathSearch;
            ClearSight = clearSight;
            AllowDiagonal = allowDiagonal;
            KeepDistance = keepDistance;
            MaxSearchDist = maxSearchDist;
            MinTargetDist = minTargetDist;
            MaxTargetDist = maxTargetDist;
        }

        public bool FullPathSearch { get; }
        public bool ClearSight { get; }
        public bool AllowDiagonal { get; }
        public bool KeepDistance { get; }
        public int MaxSearchDist { get; }
        public int MinTargetDist { get; }
        public int MaxTargetDist { get; }
    }
}
