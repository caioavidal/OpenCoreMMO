using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums.Location.Structs
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]
    public readonly struct FindPathParams
    {
      
        public FindPathParams(bool fullPathSearch, bool clearSight, bool allowDiagonal, bool keepDistance, int maxSearchDist, int minTargetDist, int maxTargetDist, bool oneStep)
        {
            FullPathSearch = fullPathSearch;
            ClearSight = clearSight;
            AllowDiagonal = allowDiagonal;
            KeepDistance = keepDistance;
            MaxSearchDist = maxSearchDist;
            MinTargetDist = minTargetDist;
            MaxTargetDist = maxTargetDist;
            OneStep = oneStep;
        }
        public FindPathParams(bool useDefault)
        {
            FullPathSearch = default;
            ClearSight = default;
            AllowDiagonal = default;
            KeepDistance = default;
            MaxSearchDist = default;
            MinTargetDist = default;
            MaxTargetDist = default;
            OneStep = default;

            if (useDefault)
            {
                FullPathSearch = true;
                ClearSight = true;
                AllowDiagonal = true;
                KeepDistance = false;
                MaxSearchDist = 12;
                MinTargetDist = 1;
                MaxTargetDist = 1;
            }

        }

        public bool FullPathSearch { get; }
        public bool ClearSight { get; }
        public bool AllowDiagonal { get; }
        public bool KeepDistance { get; }
        public bool OneStep { get; }
        public int MaxSearchDist { get; }
        public int MinTargetDist { get; }
        public int MaxTargetDist { get; }
    }
}
